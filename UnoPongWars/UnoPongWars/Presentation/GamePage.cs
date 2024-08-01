using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace UnoPongWars.Presentation;

public sealed partial class GamePage : Page
{
    private readonly Color Blue = Color.FromArgb(255, 27, 154, 249);
    private readonly Color Green = Color.FromArgb(255, 107, 227, 173);

    public object ViewModel { get; set; }

    public Color PlayerColor(Cell cell) => cell.Player == 0 ? Blue : Green;
    public Color CellColor(Cell cell) => cell.Player == 0 ? Green : Blue;

    static public GamePage singleton;


    public async Task DisplayDialogTest()
    {
        var messageDialog = new MessageDialog("No internet connection");
        messageDialog.Commands.Add(new UICommand("Option 1"));
        messageDialog.Commands.Add(new UICommand("Option 2"));

        
        var app = Application.Current as App;

        // Get the current window's HWND by passing a Window object
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(app.MainWindow);
        // Associate the HWND with the dialog
        WinRT.Interop.InitializeWithWindow.Initialize(messageDialog, hwnd);

        System.Diagnostics.Debug.WriteLine("Showing Message Dialog...");

        await messageDialog.ShowAsync();
    }

    public async Task DisplayDialogTest2() {
        ContentDialog noWifiDialog = new ContentDialog
        {
            Title = "No WiFi connection",
            Content = "Check your connection and try again.",
            CloseButtonText = "OK"
        };

        var app = Application.Current as App;

        // Make sure to set the XamlRoot!
        noWifiDialog.XamlRoot = this.XamlRoot;

        ContentDialogResult result = await noWifiDialog.ShowAsync();
    }


    public GamePage()
    {
        System.Diagnostics.Debug.WriteLine("Redraw GamePage...");

        singleton = this;
        this.DataContext<BindableGameModel>((page, vm) => page
            .NavigationCacheMode(NavigationCacheMode.Required)
            .SafeArea(SafeArea.InsetMask.All)
            .Background(Theme.Brushes.Background.Default)
            .Content(
                new Grid()
                    .Margin(20)
                    .RowDefinitions("*,Auto,Auto,Auto")
                    .Children(
                        new Viewbox()
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Child(
                                new ItemsRepeater()
                                    .ItemsSource(() => vm.Cells)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .Width(160)
                                    .Height(160)
                                    .Layout(
                                        new UniformGridLayout()
                                            .Orientation(Orientation.Vertical)
                                            .MaximumRowsOrColumns(16))
                                    .ItemTemplate<Cell>(cell => CellTemplate(cell))),
                                new TextBlock()
                                    .Text(() => vm.Score)
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .Style(Theme.TextBlock.Styles.HeadlineSmall)
                                    .Margin(10)
                                    .Grid(row: 1),
                                new Slider()
                                    .MaxWidth(400)
                                    .Margin(16)
                                    .Maximum(1000)

// Adjusting minimum slider value for Android to address the UI layer performance concerns specific to this platform
#if __ANDROID__
                                    .Minimum(100)
#else
                                    .Minimum(10)
#endif
                                    .Grid(row: 2)
                                    .Value(x => x.Binding(() => vm.Speed).TwoWay()),
                                new Button()
                                    .Content("Test")
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .Command(() => vm.testFunction)
                                    .Grid(row: 3)
            )));
    }

    private Grid CellTemplate(Cell cell)
        => new Grid()
            .Children(
                new Rectangle()
                    .Width(10)
                    .Height(10)
                    .Fill(() => cell, cell => new SolidColorBrush(CellColor(cell))),
                new Ellipse()
                    .Width(10)
                    .Height(10)
                    .Fill(() => cell, cell => new SolidColorBrush(PlayerColor(cell)))
                    .Visibility(() => cell, cell => cell.HasBall ? Visibility.Visible : Visibility.Collapsed));
}
