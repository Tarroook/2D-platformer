using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JumpSlots : MonoBehaviour
{
    private PlayerMovement pm;
    [SerializeField] private Image jumpSlot;
    [SerializeField] private List<Image> jumpSlots;

    private void OnEnable() // problem might be because pm is set in start which is called after OnEnable() can't be fucked rn lmao
    {
        StartCoroutine(subscribeToEvents());
    }

    private void OnDisable()
    {
        pm.onChangedMaxJumps -= changeMaxSlots;
        pm.onAirJump -= removeFullFeather;
        pm.onFilledJumpsLeft -= fillAllSlots;
    }

    // Start is called before the first frame update
    void Start()
    {
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        jumpSlots = new List<Image>();
        for(int i = 0; i < pm.maxAirJumps; i++)
        {
            Image currSlot = Instantiate(jumpSlot);
            currSlot.rectTransform.SetParent(gameObject.transform);
            currSlot.rectTransform.anchoredPosition = new Vector3(50 * (i + 1), -40, 0);
            jumpSlots.Add(currSlot);
        }
    }


    void removeFullFeather(int index)
    {
        jumpSlots[index].transform.Find("Full feather").gameObject.SetActive(false);
    }

    void fillAllSlots()
    {
        foreach (Image slot in jumpSlots)
        {
            slot.transform.Find("Full feather").gameObject.SetActive(true);
        }
    }
    
    void changeMaxSlots(int amount)
    {
        
    }


    private IEnumerator subscribeToEvents()
    {
        bool subscribed1 = false;
        bool subscribed2 = false;
        bool subscribed3 = false;
        while (!subscribed1 && !subscribed2 && !subscribed3)
        {
            yield return new WaitForSeconds(.1f);
            try
            {
                if (!subscribed1)
                {
                    pm.onChangedMaxJumps += changeMaxSlots;
                    subscribed1 = true;
                }
                if (!subscribed2)
                {
                    pm.onAirJump += removeFullFeather;
                    subscribed2 = true;
                }
                if (!subscribed3)
                {
                    pm.onFilledJumpsLeft += fillAllSlots;
                    subscribed3 = true;
                }
            }
            catch (System.NullReferenceException) { }
        }
    }
}
