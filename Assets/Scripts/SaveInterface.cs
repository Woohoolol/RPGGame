using UnityEngine;
//Using this interface to talk to all classes
public interface SaveInterface
{
    //No ref since just care about reading
    void loadData(GameData data);

    //Ref since care about saving the actual data
    void saveData(ref GameData data);
}