public class CheckpointManager
{
    private static CheckpointManager instance;
    public static CheckpointManager Instance
    {
        get
        {
            if (instance == null)
                instance = new CheckpointManager();
            return instance;
        }
    }

    public Checkpoint CurrentCheckpoint;

    public static void SetCheckpoint(Checkpoint checkpoint)
    {
        if (Instance.CurrentCheckpoint == checkpoint)
            return;

        Instance.CurrentCheckpoint = checkpoint;
    }

    public static void Remove(Checkpoint checkpoint)
    {
        if (Instance.CurrentCheckpoint == checkpoint)
            Instance.CurrentCheckpoint = null;
    }
}
