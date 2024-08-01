using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace UnoPongWars.Presentation;

public partial record GameModel
{
    public GameModel()
    {
        Title = "Game";

        Speed.ForEachAsync(async (speed, _) => Game.Speed = speed);
    }

    private int GetCellsCount(IImmutableList<Cell> cells, int playerId) => cells.Where(i => i.Player == playerId).Count();

    public string? Title { get; }

    Game Game { get; } = new Game(16, 16);

    public IListFeed<Cell> Cells => ListFeed.AsyncEnumerable(Game.Loop);

    public IFeed<string> Score => Cells.AsFeed().Select(cells => $"Green {GetCellsCount(cells, 0)} | Blue {GetCellsCount(cells, 1)}");

    public IState<int> Speed => State.Value(this, () => Game.Speed);

    public async Task DisplayDeleteFileDialogCommand()
    {
        var messageDialog = new MessageDialog("No internet connection");
        messageDialog.Commands.Add(new UICommand(
            "Try again"));

        //await messageDialog.ShowAsync();
        
        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        async() =>
        {
            //MessageDialog msg= new MessageDialog("No Internet");
            await messageDialog.ShowAsync();
         });

    }

    public async Task testFunction() {
        await GamePage.singleton.DisplayDialogTest();
    }
}
