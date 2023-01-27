namespace ServiceStudio.View;

public interface IView
{
    void Activate();
    string Caption { get; set; }
}
