using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;
using DG.Tweening;


public class MyYield : CustomYieldInstruction
{

   

    public override bool keepWaiting
    {
        get
        {
            return !Input.GetMouseButtonDown(1);
        }
    }
}

public class Test : MonoBehaviour {

    public Text show;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform cardListRectTrf;


    private List<Card> cardsObjects = new List<Card>();
    private void Start()
    {
        SetCardList();
        //Sequence seq = DOTween.Sequence();

        //seq.Append(show.rectTransform.DOScale(1.1f, 0.2f));
        //seq.Append(show.rectTransform.DOScale(1f, 0.2f));
        //seq.Append(show.rectTransform.DOLocalMoveY(100,0.5f).SetRelative());
        //seq.Insert(0.4f, show.DOFade(0, 0.6f).SetEase(Ease.InOutQuad));
        //seq.SetLoops(-1, LoopType.Restart);
      //  show.rectTransform.DOScale(1.5f, 0.2f).OnComplete(()=> show.rectTransform.DOScale(1f, 0.5f));
     //   show.DOFade(0, 3f).SetEase(Ease.InBack).SetLoops(-1, LoopType.Yoyo);



    }


    public void SetCardList()
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            Card card = Instantiate(cardPrefab, cardListRectTrf).GetComponent<Card>();
            cardsObjects.Add(card);
        }
    }


    public void OnPlayButtonClick()
    {
        
    }


    private void Update()
    {
       
    }

    private IEnumerator MyCoroutine()
    {
        Debug.Log("enter corountine");
        yield return new MyYield();
        Debug.Log("leave corountine");
    }
}
