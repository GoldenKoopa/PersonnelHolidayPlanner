namespace LeaveCli.Views;

public interface View {
    void renderView();
    void handleKeys(ConsoleKeyInfo keyInfo);
}
