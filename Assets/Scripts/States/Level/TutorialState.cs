using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TutorialState : State<LevelStateManager>
{
    private float _elapsedTime = 0;
    private readonly float _cooldown = 6f;
    private readonly TutorialStep _currentStep;
    public TutorialState(LevelStateManager context) : base(context)
    {
    }

    public override void EnterState()
    {
        // Enable input to allow player to interact with the game
        InputRouter.Gate.SetEnabled(true);


      
    }
    private void OnBoardSetupComplete(IBoardPresenter board)
    {
        UpdateTutorialText(_currentStep);
        BoardPresenter.OnBoardSetupComplete -= OnBoardSetupComplete;
    }
    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
        
        if (_elapsedTime > _cooldown)
        {
            _elapsedTime = 0;
            CoroutineHandler.StartStaticCoroutine(ShowPointer(_currentStep));
        }

    }
    private void UpdateTutorialText(TutorialStep step)
    {
        // TODO: Update tutorial text
    }



    public IEnumerator ShowPointer(TutorialStep step)
    {
        if (step == null)
        {
            Debug.LogWarning("[TutorialState] Step is null");
            yield break;
        }
        // TODO: Show pointer
    

    }

    public override void ExitState()
    {
        context.Tutorial.StopTutorial();

    }
    
}