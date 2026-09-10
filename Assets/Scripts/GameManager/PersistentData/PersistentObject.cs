using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentObject<TPersist> : 
    MonoBehaviour where TPersist : PersistentDataBase
{
    protected TPersist Persist { get; set; }
    protected TPersist RetrieveData(TPersist persist)
    {
        TPersist temp = (TPersist)PersistentData.RetrieveDataContainer(persist.GetType().ToString());
        if(temp == null)
        {
            PersistentData.AddDataContainer(persist);
            return persist;
        }
        else
        {
            return temp;
        }
    }

    protected abstract void PullData();
    protected abstract void PushData();
}
