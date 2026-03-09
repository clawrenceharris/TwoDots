using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Orchestrates the fillstep cascade: on connection complete, switches to CascadeState (input off),
/// runs pre-gravity steps (connection clear, seed, hedgehog), then gravity and refill, then post-fill
/// steps (anchor, lotus, gem). Repeats until no new work; then restores the previous state (input on).
/// Subscribes to ConnectionPresenter.OnConnectionCompleted; auto-creates an instance if none exists.
/// </summary>
public class CascadeRunner : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        if (FindFirstObjectByType<CascadeRunner>() != null) return;
        var runnerObject = new GameObject("CascadeRunner");
        runnerObject.AddComponent<CascadeRunner>();
    }

    [SerializeField] private bool _enableTrace;
    [SerializeField] private int _maxCascadeIterations = 50;

    private IBoardPresenter _board;
    private LevelStateManager _stateManager;
    private IState _previousState;
    private bool _isRunning;

    private CascadeContext _context;
    private readonly FillStepQueue _preGravityQueue = new();
    private readonly FillStepQueue _postFillQueue = new();
    private readonly List<IFillStepProducer> _preGravityProducers = new();
    private readonly List<IFillStepProducer> _postFillProducers = new();
    private int _stepSequence;

    private void Awake()
    {
        _board = FindFirstObjectByType<BoardPresenter>();
        _stateManager = FindFirstObjectByType<LevelStateManager>();
        BuildProducers();
    }

    private void OnEnable()
    {
        ConnectionPresenter.OnConnectionCompleted += HandleConnectionCompleted;
    }

    private void OnDisable()
    {
        ConnectionPresenter.OnConnectionCompleted -= HandleConnectionCompleted;
    }

    private void HandleConnectionCompleted(ConnectionResult payload)
    {
        if (_isRunning) return;
        if (payload == null || payload.DotIdsInPath == null || payload.DotIdsInPath.Count < 2) return;
        StartCascade(payload);
    }

    public void StartCascade(ConnectionResult payload)
    {
        if (_isRunning) return;
        if (_stateManager == null)
        {
            Debug.LogError("[CascadeRunner] Missing LevelStateManager.");
            return;
        }
        
        if (_board == null)
        {
            Debug.LogError("[CascadeRunner] Missing BoardPresenter.");
            return;
        }

        _context = new CascadeContext(_board, payload);
        _previousState = _stateManager.CurrentState;
        _stateManager.ChangeState(new CascadeState(_stateManager));
        

        StartCoroutine(RunCascade());
    }

    private IEnumerator RunCascade()
    {
        _isRunning = true;
        _stepSequence = 0;
        _preGravityQueue.Clear();
        _postFillQueue.Clear();

        bool anyWork = true;
        int iterations = 0;

        while (anyWork && iterations < _maxCascadeIterations)
        {
            anyWork = false;
            iterations++;
            _context.ChainIndex = iterations;

            EnqueueProducerSteps(_preGravityProducers, _preGravityQueue);
            bool preGravityWorked = false;
            yield return ProcessQueue(_preGravityQueue, _preGravityProducers, () => preGravityWorked = true);
            anyWork |= preGravityWorked;

            var gravityDrops = _board.CollectGravityDrops();
            if (gravityDrops.Count > 0)
            {
                anyWork = true;
                yield return PlayDrops(gravityDrops);
            }

            var refillDrops = _board.CollectRefillDrops();
            if (refillDrops.Count > 0)
            {
                anyWork = true;
                yield return PlayDrops(refillDrops);
            }

            EnqueueProducerSteps(_postFillProducers, _postFillQueue);
            bool postFillWorked = false;
            yield return ProcessQueue(_postFillQueue, _postFillProducers, () => postFillWorked = true);
            anyWork |= postFillWorked;
        }

        if (iterations >= _maxCascadeIterations)
        {
            Debug.LogError("[CascadeRunner] Cascade aborted due to iteration guard.");
        }

        FinishCascade();
    }

    private void FinishCascade()
    {
        if (_stateManager != null)
        {
            _stateManager.ChangeState(_previousState);
        }
        _previousState = null;
        _isRunning = false;
        
    }

    private void BuildProducers()
    {
        _preGravityProducers.Clear();
        _postFillProducers.Clear();

        _preGravityProducers.Add(new ConnectionClearProducer());
        _preGravityProducers.Add(new SeedAdjacencyProducer());
        _preGravityProducers.Add(new HedgehogProducer());
        _postFillProducers.Add(new BombProducer());
        _postFillProducers.Add(new AnchorSinkProducer());
        _postFillProducers.Add(new LotusProducer());
        _postFillProducers.Add(new GemProducer());
    }

    private void EnqueueProducerSteps(List<IFillStepProducer> producers, FillStepQueue queue)
    {
        if (producers == null || queue == null) return;
        var steps = new List<FillStep>();
        foreach (var producer in producers)
        {
            producer?.CollectSteps(_context, steps);
        }

        foreach (var step in steps)
        {
            queue.Enqueue(step, ref _stepSequence);
        }
    }

    private IEnumerator ProcessQueue(
        FillStepQueue queue,
        List<IFillStepProducer> producers,
        System.Action markWork)
    {
        while (queue.TryDequeue(out var step))
        {
            markWork?.Invoke();
            TraceStep(step);

            var hitAnimations = ExecuteHitPhase(step);
            if (hitAnimations.Count > 0)
            {
                yield return WaitForAnimations(hitAnimations);
            }

            var result = ExecuteClearPhase(step, out var clearAnimations);
            if (result.HasClears)
            {
                _context.SetRecentClears(result.ClearedDotIds, result.ClearedPositions);
            }
            if (clearAnimations.Count > 0)
            {
                yield return WaitForAnimations(clearAnimations);
            }
            if (result.HasClears)
            {
                EnqueueProducerSteps(producers, queue);
            }
        }
        foreach (var id in _context.ClearedDotIds)
        {
            _board.RemoveAndDestroyDot(id);
        }
        _context.ClearRecentClears();

    }

    private List<Sequence> ExecuteHitPhase(FillStep step)
    {
        var animations = new List<Sequence>();
        if (step == null) return animations;

        if (step.ToHit.Count > 0)
        {
            var hittables = _board.CollectPresenters<IHittableDotPresenter>(new List<string>(step.ToHit));
            foreach (var hittable in hittables)
            {
                if (_board.TryHitDot(hittable.Dot.ID, out _))
                {
                    var sequence = hittable.Hit();
                    if (sequence != null)
                        animations.Add(sequence);
                }
            }
        }

        if (step.ToExplode.Count > 0)
        {
            var explodables = _board.CollectPresenters<IExplodableDotPresenter>(new List<string>(step.ToExplode));
            foreach (var explodable in explodables)
            {
                explodable.PrepareForExplode(new List<string>(step.ToHit), new List<string>(step.ToExplode));
            }
            foreach (var explodable in explodables)
            {
                Debug.Log($"explodable: {explodable.Dot.ID}");
                var sequence = explodable.Explode();
                if (sequence != null)
                    animations.Add(sequence);
            }
        }

        return animations;
    }

    private FillStepResult ExecuteClearPhase(FillStep step, out List<Sequence> animations)
    {
        animations = new List<Sequence>();
        if (step == null) return FillStepResult.Empty;

        var clearCandidates = new HashSet<string>(step.ToClear);
        clearCandidates.UnionWith(step.ToHit);

        if (clearCandidates.Count == 0) return FillStepResult.Empty;

        var clearedIds = new List<string>();
        var clearedPositions = new List<Vector2Int>();

        var clearables = _board.CollectPresenters<IClearableDotPresenter>(new List<string>(clearCandidates));
        foreach (var clearable in clearables)
        {
            if (_board.TryClearDot(clearable.Dot.ID))
            {
                var sequence = clearable.Clear();
                if (sequence != null)
                    animations.Add(sequence);

                clearedIds.Add(clearable.Dot.ID);
                clearedPositions.Add(clearable.Dot.GridPosition);
            }
        }

        foreach (var id in clearedIds)
            _context.ClearedDotIds.Add(id);

        return new FillStepResult(clearedIds, clearedPositions);
    }
   
    private IEnumerator PlayDrops(List<BoardPresenter.DotDrop> drops)
    {
        if (drops == null || drops.Count == 0) yield break;

        int remaining = 0;
        foreach (var drop in drops)
        {
            if (drop.Presenter == null) continue;
            remaining++;
            var captured = drop.Presenter;
            captured.OnDotDropped += HandleDropComplete;
        }

        foreach (var drop in drops)
        {
            if (drop.Presenter == null) continue;
            drop.Presenter.Drop(drop.TargetRow);
        }

        if (remaining > 0)
            yield return new WaitUntil(() => remaining == 0);

        void HandleDropComplete(string dotId)
        {
            var presenter = _board.GetDot(dotId);
            if (presenter == null) return;
            presenter.OnDotDropped -= HandleDropComplete;
            remaining--;
        }
    }

    private IEnumerator WaitForAnimations(List<Sequence> animations)
    {
        if (animations == null || animations.Count == 0) yield break;

        int remaining = 0;
        foreach (var sequence in animations)
        {
            if (sequence == null) continue;
            remaining++;
            sequence.OnComplete(() => remaining--);
        }

        if (remaining > 0)
            yield return new WaitUntil(() => remaining == 0);
    }

    private void TraceStep(FillStep step)
    {
        if (!_enableTrace || step == null) return;
        Debug.Log($"[Cascade] {step.Phase} {step.Type} ({step.ToClear.Count}) src={step.Source}");
    }
}
