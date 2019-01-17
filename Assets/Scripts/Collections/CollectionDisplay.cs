using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Engine;

public class CollectionDisplay : MonoBehaviour
{
    public CollectionType type;
    public Image image;
    public Coroutine coroutine;
    [HideInInspector]public CanvasGroup group;
    int showTime = 3;
    int ammount = 0;
    Text text;
    float scale = 1;
    static readonly Vector3 scaleVector = new Vector3(1, 1, 1);

    public void Awake()
    {
        image = GetComponentInChildren<Image>();
        group = GetComponent<CanvasGroup>();
        text = GetComponentInChildren<Text>();
        enabled = false;
    }

    private void Start()
    {
        Level.LevelLoaded += Clear;
    }

    void Clear()
    {
        ammount = 0;
    }

    private void OnDestroy()
    {
        Level.LevelLoaded -= Clear;
    }

    public void ShowDisplay()
    {
        enabled = true;
        showTime = 3;
        ammount++;
        scale = 1.2f;
        text.text = "x" + ammount;
        if (coroutine == null)
        {
            if (CollectionDisplayManager.Instance.isDisplaying)
            {
                CollectionDisplayManager.Instance.StopDisplaying();
            }
            CollectionDisplayManager.Instance.isDisplaying = true;
            coroutine = StartCoroutine(Activate());
        }
    }

    IEnumerator Activate()
    {
        while (group.alpha < 1)
        {
            group.alpha += 0.1f;
            yield return null;
        }

        while (showTime > 0)
        {
            showTime--;
            yield return new WaitForSeconds(1);
        }
        
        while (group.alpha > 0)
        {
            group.alpha -= 0.1f;
            yield return null;
        }
        coroutine = null;
        enabled = false;
        CollectionDisplayManager.Instance.isDisplaying = false;
        yield return null;
    }

    private void Update()
    {
        if (scale > 1)
        {
            transform.localScale = scaleVector * scale;
            scale -= 0.05f;
        }
    }
}
