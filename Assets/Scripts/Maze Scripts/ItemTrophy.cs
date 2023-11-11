public class ItemTrophy : PickUp
{
    protected override void OnPickUp()
    {
        base.OnPickUp();
        if (GameManager.instance != null)
        {
            GameManager.instance.EndGame(true);
        }
    }
}
