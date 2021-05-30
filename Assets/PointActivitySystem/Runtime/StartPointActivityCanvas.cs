using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PointActivitySystem.Runtime
{
    public class StartPointActivityCanvas : MonoBehaviour
    {
        public static StartPointActivityCanvas StartPointActivityCanvasInstance;

        [SerializeField] private Image pointIconImage;
        [SerializeField] private Button startPointActivity;
        [SerializeField] private Button closeCanvasButton;

        [Space, SerializeField] private TMP_Text pointInstructionText;
        [SerializeField] private Canvas pointInstructionView;
        private Canvas attachedCanvas;

        private void Awake ()
        {
            if (StartPointActivityCanvasInstance == null)
            {
                StartPointActivityCanvasInstance = this;
            }
            else
                Destroy (this);
        }

        private void OnEnable ()
        {
            closeCanvasButton.onClick.AddListener (CloseCanvas);
        }

        private void OnDisable ()
        {
            closeCanvasButton.onClick.RemoveAllListeners ();
            startPointActivity.onClick.RemoveAllListeners ();
        }

        private void Start ()
        {
            attachedCanvas = GetComponent <Canvas> ();
        }


        public void InjectPointDataAndInitialize (Sprite icon, RoutePoint routePoint, string pointInstructionText = "")
        {
            startPointActivity.onClick.RemoveAllListeners ();

            this.pointInstructionText.text = pointInstructionText;

            pointInstructionView.enabled  = !string.IsNullOrEmpty (pointInstructionText);


            pointIconImage.sprite = icon;


            attachedCanvas.enabled =true ;


            startPointActivity.onClick.AddListener (delegate
            {
                Instantiate (routePoint.AttachedPoint.PointPrefab);
                attachedCanvas.enabled =false;
            });
        }

        private void CloseCanvas ()
        {
            attachedCanvas.enabled =false;
        }
    }
}