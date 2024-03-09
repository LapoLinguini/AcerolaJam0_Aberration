public interface IInteractable
{
    bool _isInteractable { get; set; }
    void Interactable(bool _isInteractable);
    void Interact();
}
