// This is an Interface — not a MonoBehaviour
// Any puzzle object that needs to reset between loops implements this
// LevelManager calls ResetObject() on all of them at once
// This keeps LevelManager clean — it doesn't need to know about doors or buttons specifically

public interface IResettable
{
    // Every class that uses this interface MUST have a ResetObject() method
    void ResetObject();
}