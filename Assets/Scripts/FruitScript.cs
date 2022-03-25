using System.Collections;
using UnityEngine;

public class FruitScript : MonoBehaviour
{
    [SerializeField] private float dragSpeed = 20;
    [SerializeField] private float returnSpeed = 10;
    private Vector3 dragOffset;
    private Camera cam;
    private CircleCollider2D col;

    private GameManagerScript GMS;

    private void Awake()
    {
        cam = Camera.main;
        col = GetComponent<CircleCollider2D>();
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    private void OnMouseDown()
    {
        col.enabled = false;
        dragOffset = transform.position - GetMousePos();
    }

    private void OnMouseDrag()
    {
        transform.position = Vector3.MoveTowards(transform.position, GetMousePos() + dragOffset, dragSpeed * Time.deltaTime);
    }

    private void OnMouseUp()
    {
        Vector2 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider == null)
        {
            SoundManager.instance.PlaySingle(1f, SoundManager.instance.noSound);
            StartCoroutine(GMS.MoveAnimationCoroutine(gameObject, GMS.startPlaceT.position));
        }
        else
        {
            StartCoroutine(RightSlotCoroutine());
        }
    }

    private Vector3 GetMousePos()
    {
        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    private IEnumerator RightSlotCoroutine()
    {
        SoundManager.instance.PlaySingle(1f, SoundManager.instance.itemSounds[1]);
        GMS.workingMount.GetComponent<CircleCollider2D>().enabled = false;
        Vector2 endPos = GMS.workingMount.transform.position;
        Vector2 transformV2 = transform.position;
        while (transformV2 != endPos)
        {
            transform.position = Vector2.MoveTowards(transformV2, endPos, returnSpeed * Time.deltaTime);
            transformV2 = transform.position;
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, .05f);
        LeanTween.scale(gameObject, new Vector3(1.7f, 1.7f, 1), .3f);
        yield return new WaitForSeconds(.3f);
        LeanTween.scale(gameObject, Vector3.one, .3f);
        GMS.workingMount = null;
        GMS.NextFruit();
    }

}
