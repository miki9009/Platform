using UnityEngine;
using Objectives;
using UnityEngine.UI;
using System.Collections;

public class ObjectivePanel : MonoBehaviour
{
    public Image progressBar;
    public Image circle;
    public Text title;
    public Objective objective;
    public GameObject ok;
    public GameObject failed;
    public Effect finishedEffect;
    public Effect failedEffect;
    public Image timer;

    public Color colOptional;
    public Color colFailed;
    public Color colCompleted;

    LayoutElement layoutElement;

    CollectionObjective timerObjective;

    public void Initialize()
    {
        objective.ProgressUpdated += OnUpdate;
        objective.Finished += OnFinished;
        layoutElement = GetComponent<LayoutElement>();
        if(objective.optional)
        {
            circle.color = colOptional;
        }
        if(objective.GetType() == typeof(CollectionObjective))
        {
            timerObjective = (CollectionObjective)objective;
            if (timerObjective.isTimer)
            {
                timerObjective.ClockUpdate += OnTimeUpdate;
                timer.enabled = true;
            }
        }
    }

    void OnUpdate(Objective objective)
    {
        progressBar.fillAmount = objective.Progress;
    }

    void OnTimeUpdate()
    {
        timer.fillAmount = timerObjective.time/timerObjective.startTimer;
    }

    private void OnDestroy()
    {
        if(objective != null)
        {
            objective.ProgressUpdated -= OnUpdate;
            objective.Finished -= OnFinished;
        }
        if (timerObjective != null)
        {
            timerObjective.ClockUpdate -= OnTimeUpdate;
        }
    }

    void OnFinished(Objective objective)
    {
        progressBar.gameObject.SetActive(false);
        timer.enabled = false;
        if (objective.state == State.Completed)
        {
            title.color = Color.green;
            ok.SetActive(true);
            finishedEffect.Play(); 
        }
        else
        {
            title.color = Color.red;
            failed.SetActive(true);
            failedEffect.Play();
        }
        StartCoroutine(MovePanel());
    }

    IEnumerator MovePanel()
    {
        yield return new WaitForSeconds(3);

        while(layoutElement.minHeight > 0)
        {
            layoutElement.minHeight -= 2;
            yield return null;
        }
    }
}