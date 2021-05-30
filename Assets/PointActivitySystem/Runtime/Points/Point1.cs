using PointActivitySystem.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point1 : Point
{
    [SerializeField] private Button endPointActivityButton;
    [SerializeField] private Button completePointButton;


    // Start is called before the first frame update
    void Start()
    {
        endPointActivityButton.onClick.AddListener(EndPointActivity);
        completePointButton.onClick.AddListener(SetComplete);
    }

    protected override void StartPointActivity()
    {
        base.StartPointActivity();
    }

    protected override void SetComplete()
    {
        base.SetComplete();
        Debug.Log("POINT COMPLETED");
    }
}
