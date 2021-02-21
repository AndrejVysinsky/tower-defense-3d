using System;

[Serializable]
public class SaveableCheckpoint : SaveableObject
{
    public int checkpointNumber;

    public SaveableCheckpoint()
    {

    }

    public SaveableCheckpoint(SaveableObject saveableObject, int number) : base(saveableObject)
    {
        checkpointNumber = number;
    }
}
