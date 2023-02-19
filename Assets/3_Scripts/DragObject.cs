using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    public RectTransform playListRect;
    public GameObject playList;
    public GameObject songPlaceHolder;
    private GameObject draggedSong;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("Begin Drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        transform.GetChild(0).gameObject.SetActive(false);
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("Dragging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("End Drag");
        transform.SetParent(parentAfterDrag);
        transform.GetChild(0).gameObject.SetActive(true);
        image.raycastTarget = true;
        //songPlaceHolder.gameObject.SetActive(false);
    }

}
