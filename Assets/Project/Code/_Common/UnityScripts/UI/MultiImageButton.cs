using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class MultiImageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
	[SerializeField]
	private Image[] _affectedImages;
    public Image[] AffectedImages
    {
        get
        {
            return _affectedImages;
        }
    }

    private Button _myButton;

	public void Awake() {
		_myButton = GetComponent<Button>();
	}

    public void SetEnabled(bool enabled)
    {
        _myButton.image.enabled = enabled;
        for (int i = 0; i < AffectedImages.Length; i++)
            AffectedImages[i].enabled = enabled;
    }

    public void AddChildImages(GameObject parentObject)
    {
        Image[] images = new Image[0];
        _affectedImages = AddChildImages(parentObject.transform, images);
    }

    Image[] AddChildImages(Transform parentTransform, Image[] allImages)
    {
        Image[] childImages = AddImage(allImages, null);
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform childObject = parentTransform.GetChild(i);
            Image image = childObject.GetComponent<Image>();
            if (image != null)
            {
                childImages = AddImage(childImages, image); 
            }
            childImages = AddChildImages(childObject, childImages);
        }
        return childImages;
    }

    Image[] AddImage(Image[] images, Image image)
    {
        Image[] newImages = new Image[image != null ? images.Length + 1 : images.Length];
        for (int i = 0; i < images.Length; i++)
            newImages[i] = images[i];
        if (image != null)
            newImages[newImages.Length - 1] = image;
        return newImages;
    }

	public void OnPointerEnter(PointerEventData eventData) {
		if (!_myButton.interactable) {
			return;
		}

		for (int i = 0; i < _affectedImages.Length; i++) {
            _affectedImages[i].CrossFadeColor(_myButton.colors.highlightedColor, _myButton.colors.fadeDuration, true, true);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (!_myButton.interactable) {
			return;
		}

		for (int i = 0; i < _affectedImages.Length; i++) {
            _affectedImages[i].CrossFadeColor(_myButton.colors.normalColor, _myButton.colors.fadeDuration, true, true);
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (!_myButton.interactable) {
			return;
		}

		for (int i = 0; i < _affectedImages.Length; i++) {
            _affectedImages[i].CrossFadeColor(_myButton.colors.pressedColor, _myButton.colors.fadeDuration, true, true);
		}
	}

	public void OnPointerUp(PointerEventData eventData) {
		if (!_myButton.interactable) {
			return;
		}

		for (int i = 0; i < _affectedImages.Length; i++) {
            _affectedImages[i].CrossFadeColor(_myButton.colors.highlightedColor, _myButton.colors.fadeDuration, true, true);
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (!_myButton.interactable) {
			return;
		}

		//unimplemented
	}
}
