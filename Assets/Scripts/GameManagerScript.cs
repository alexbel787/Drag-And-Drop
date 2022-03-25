using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public List<GameObject> mountsList;
    public GameObject[] mountPlaces;
    public List<GameObject> fruitsList;
    [HideInInspector]
    public Transform startPlaceT;
    public GameObject workingMount;
    public struct Mount
    {
        public GameObject mountObj;
        public string mountName;
    }
    private List<Mount> mountStructList = new List<Mount>();

    private void Start()
    {
        if (((float)Screen.width / Screen.height) < 1.7f)
        {
            var CCW = Camera.main.GetComponent<CameraConstantWidth>();
            CCW.enabled = true;
            CCW.WidthOrHeight = 1f;
            //GameObject.Find("Canvas").GetComponent<CanvasScaler>().matchWidthOrHeight = .6f;
        }
        startPlaceT = GameObject.Find("Environment/StartPlace").transform;
        StartCoroutine(StartLevelCoroutine());
    }

    private IEnumerator StartLevelCoroutine()
    {
        int count = mountsList.Count - 1;
        for (int i = 0; i <= count; i++)
        {
            GameObject mount = mountsList[Random.Range(0, mountsList.Count)];
            SoundManager.instance.PlaySingle(.7f, SoundManager.instance.fallingSounds[0]);
            StartCoroutine(MoveAnimationCoroutine(mount, mountPlaces[i].transform.position));
            mountsList.Remove(mount);
            mountStructList.Add(new Mount { mountObj = mountPlaces[i], mountName = mount.name.Replace("Mount", "") });
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(.5f);
        NextFruit();
    }

    public IEnumerator MoveAnimationCoroutine(GameObject obj, Vector3 endPos)
    {
        obj.GetComponent<SpriteRenderer>().enabled = true;
        int rnd = 1;
        if (Random.Range(0, 2) == 1) rnd = -1;
        Vector3 perpDir = Vector3.Cross((endPos - obj.transform.position).normalized, Vector3.forward * rnd);
        Vector3 offset = (obj.transform.position + endPos) / 2 + perpDir;
        float time = Vector2.Distance(obj.transform.position, endPos) / 6;
        LeanTween.move(obj, new Vector3[] { obj.transform.position, offset, offset, endPos }, time);
        LeanTween.rotateAround(obj, Vector3.forward, 360f, time);
        yield return new WaitForSeconds(time);
        if (obj.CompareTag("Fruit"))
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
            obj.GetComponent<CircleCollider2D>().enabled = true;
            if (workingMount == null) 
                foreach (Mount m in mountStructList)
                    if (obj.name == m.mountName)
                    {
                        workingMount = m.mountObj;
                        workingMount.GetComponent<CircleCollider2D>().enabled = true;
                        break;
                    }
        }
        SoundManager.instance.PlaySingle(1f, SoundManager.instance.itemSounds[0]);
    }

    public void NextFruit()
    {
        if (fruitsList.Count > 0)
        {
            SoundManager.instance.PlaySingle(1f, SoundManager.instance.fallingSounds[1]);
            GameObject fruit = fruitsList[Random.Range(0, fruitsList.Count)];
            StartCoroutine(MoveAnimationCoroutine(fruit, startPlaceT.position));
            fruitsList.Remove(fruit);
        }
        else StartCoroutine(NextLevelCoroutine());
    }

    private IEnumerator NextLevelCoroutine()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }
}
