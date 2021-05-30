using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace PointActivitySystem.Runtime
{
	/// <summary>
	/// Script attached to each icon that appears on map / ui and is meant to open an point panel
	/// Stores information on the point
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class RoutePoint : MonoBehaviour
	{
		[SerializeField] private PointActivity attachedPoint;
		public PointActivity AttachedPoint => attachedPoint;

		private Button attachedButton;

		#region sprite variables

		[Header("Sprites")] [SerializeField] private Image routePointBG;

		[SerializeField] private Sprite unavailableSprite;

		[SerializeField] private Sprite availableSprite;

		[SerializeField] private Sprite completedAndUnavailableSprite;

		[SerializeField] private Sprite completedSprite;

		#endregion

		private TweenerCore<Vector3, Vector3, VectorOptions> scaleTween;

		private bool working;


		private void OnEnable()
		{
			//subscribe events
			attachedPoint.PointStatusChange += SetPointStatus;
			attachedPoint.QuizCompleted += RefreshPointIconBg;

			//fetch components
			attachedButton = GetComponent<Button>();

			var canvas = GetComponent<Canvas>();

			canvas.worldCamera = UnityEngine.Camera.main;
#if !UNITY_EDITOR
        attachedButton.interactable = false;
#endif


			SetPointStatus(AttachedPoint.IsInUse);
		}

		private void OnDisable()
		{
			//unsubscribe events
			attachedPoint.PointStatusChange -= SetPointStatus;
			attachedPoint.QuizCompleted -= RefreshPointIconBg;
		}

		private void SetPointStatus(bool available)
		{
			if (available)
			{
				MakeAvailable();
			}
			else
			{
				MakeUnavailable();
			}
		}

		/// <summary>
		/// Make routepoint available
		/// </summary>
		private void MakeAvailable()
		{
			SetRoutePointSprite(attachedPoint.IsCompleted ? completedSprite : availableSprite);

			if (working)
				return;

			//Scale route point bigger when player is near enough the point
			scaleTween = transform
						 .DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f)
						 .From(Vector3.one);


			//when tween is active set working true to prevent multiple tweenings 
			scaleTween.OnStart(() => working = true);
			scaleTween.OnComplete(() => working = false);


			transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			attachedButton.interactable = true;

			attachedButton.onClick.AddListener(delegate
			{
				StartPointActivityCanvas.StartPointActivityCanvasInstance.InjectPointDataAndInitialize(
					attachedPoint.PointIconWithBackground,
					this,
					attachedPoint.PointInstructionText
				);
			});
		}


		/// <summary>
		/// Make route point unavailable
		/// </summary>
		private void MakeUnavailable()
		{
			SetRoutePointSprite(attachedPoint.IsCompleted ? completedAndUnavailableSprite : unavailableSprite);


			Transform routePointTransform;
			scaleTween = (routePointTransform = transform).DOScale(new Vector3(1, 1, 1), 0.5f).From(transform.localScale);
			routePointTransform.localScale = new Vector3(1, 1, 1);


			attachedButton.interactable = false;
		}


		/// <summary>
		/// switch route point sprite
		/// </summary>
		/// <param name="newSprite"></param>
		private void SetRoutePointSprite(Sprite newSprite)
		{
			routePointBG.sprite = newSprite;
		}

		private void RefreshPointIconBg()
		{
			if (attachedPoint.IsInUse)
				SetRoutePointSprite(attachedPoint.IsCompleted ? completedSprite : availableSprite);
			else
			{
				SetRoutePointSprite(attachedPoint.IsCompleted ? completedAndUnavailableSprite : unavailableSprite);
			}
		}
	}
}