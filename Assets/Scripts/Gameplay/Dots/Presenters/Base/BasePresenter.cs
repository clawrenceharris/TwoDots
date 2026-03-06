public class BasePresenter : IPresenter
{
    protected readonly IDotPresenter _dot;
    protected readonly IBoardPresenter _board;
    public Dot Dot => _dot.Dot;
    public DotView View => _dot.View;
    public BasePresenter(IDotPresenter dot, IBoardPresenter board)
    {
        _dot = dot;
        _board = board;
    }
}