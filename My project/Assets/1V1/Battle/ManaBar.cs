using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    [SerializeField] GameObject mana;

    public void SetMana(float manaNormalized)
    {
        mana.transform.localScale = new Vector3(manaNormalized, 1f);
    } 

    public IEnumerator SetManaSmooth(float newMp)
    {
        float curMp = mana.transform.localScale.x;
        float changeAmt = curMp - newMp;

        while(curMp - newMp > Mathf.Epsilon)
        {
            curMp -= changeAmt * Time.deltaTime;
            mana.transform.localScale = new Vector3(curMp, 1f);
            yield return null;
        }
        mana.transform.localScale = new Vector3(newMp, 1f);
    }
}
