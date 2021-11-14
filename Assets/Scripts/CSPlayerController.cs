using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CSPlayerController : MonoBehaviour
{
    private static CSPlayerController instance;
    public static CSPlayerController Instance => instance;

    [Header("Fire Interface")]
    public ShootGunController gun;
    [SerializeField] private GameObject GunObj;

    [Header("AIM")]
    [SerializeField] private Image aimVisual;
    [SerializeField] private Transform fireTarget;
    [SerializeField] private float AIMMoveSpeed = 5f;
    private Vector3 firstTargetPos;
    public void FirstTargetPos(Vector3 vec)
    {
        firstTargetPos = vec;
    }

    [Header("Nomber Chose")]
    [SerializeField] private NumericalAggregator choseNomberPanel;
    [SerializeField] private Animator cameraAnim;
    [SerializeField] private Animator backgroundAnim;
    private bool canFire = false;
    private int fireObjCount = 15;

    [Header("Panels")]
    [SerializeField] private GameObject fireUIPanel;
    [SerializeField] private Text timerText;
    [SerializeField] private Animator timerAnim;

    private bool myMove = false;
    private bool chosing = false;

    private bool choseMode = false;
    private bool fireOpen = false;

    private void Awake()
    {
        instance = this;
        gun.Init(fireTarget);

        fireUIPanel.SetActive(true);

        for (int i = 0; i < 25; i++)
        {
            GameObject go = Instantiate(VisualFireObj);
            go.SetActive(true);
            FOObjPool.Add(go);
        }
    }

    public void StartPlayQueue(int round)
    {
        if (round == 0)
        {
            choseNomberPanel.gameObject.SetActive(true);
            fireObjCount = 0;
            choseNomberPanel.StartSelection();
            myMove = true;
            chosing = true;
            choseMode = true;
            fireOpen = false;
        }
        else
        {
            myMove = true;
            chosing = false;
            fireObjCount = 0;
            LevelController.Instance.InitCastle();
            FOText.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 1, Camera.main.transform.position.z + 10);
            FOText.transform.rotation = Camera.main.transform.rotation;
            FOText.text = $"+{gun.startFireObjCount}";
            FOText.GetComponent<Animator>().SetTrigger("Start");
            StartCoroutine(FireObjCreate(25));
            StartCoroutine(NextRoundStart());
        }
    }

    private IEnumerator NextRoundStart()
    {
        yield return new WaitForSeconds(1.5f);
        canFire = true;
    }

    private IEnumerator TimerChoseEnd()
    {
        fireUIPanel.SetActive(false);
        timerAnim.SetTrigger("Play");
        float time = 10;

        while (time > 0)
        {
            if (!choseMode) yield break;

            time -= Time.deltaTime;

            if (time >= 9)
                timerText.text = $"00:10";
            else
                timerText.text = $"00:0{Mathf.CeilToInt(time)}";
            yield return new WaitForEndOfFrame();
        }

        choseMode = false;
        choseNomberPanel.StopSelection();
    }

    public void StartGameStopSelection()
    {
        choseMode = false;
        choseNomberPanel.StopSelection();
    }

    public void InputActive()
    {
        if (!StaticGameController.Instance.gameIsPlayed || !myMove) return;

        if (chosing)
        {

        }
        else
        {
            //если стреляем
            if (!fireOpen && canFire)
            {
                fireOpen = true;
                aimVisual.gameObject.SetActive(true);

                fireTarget.position = new Vector3(firstTargetPos.x, firstTargetPos.y + 2, firstTargetPos.z);
                GunObj.transform.LookAt(fireTarget);
                aimVisual.transform.position = Camera.main.WorldToScreenPoint(fireTarget.position);

                gun.Fire(fireObjCount, false);
            }
        }
    }

    private TMPro.TextMeshPro FOText;
    [SerializeField] private GameObject VisualFireObj;
    private List<GameObject> FOObjPool = new List<GameObject>();

    public void StopNumerical(int count, TMPro.TextMeshPro text)
    {
        FOText = text;
        gun.SetFireObjText(count);
        fireObjCount = count;
        chosing = false;

        StartCoroutine(StopNimAnim());
    }

    private IEnumerator StopNimAnim()
    {
        StartCoroutine(FireObjCreate(fireObjCount));

        if (cameraAnim != null) cameraAnim.SetTrigger("StartFire");
        if (backgroundAnim != null) backgroundAnim.SetTrigger("Start");

        yield return new WaitForSeconds(2);
        canFire = true;
        StaticGameController.Instance.ActivateGameUI();
    }

    private bool upMagazineGun = false;
    private IEnumerator FireObjCreate(int _count)
    {
        yield return new WaitForSeconds(0.9f);

        int foCount = Mathf.Clamp(_count, 0, 25);
        upMagazineGun = true;
        for (int i = 0; i < foCount; i++)
        {
            FOText.text = (int.Parse(FOText.text) - 1).ToString();
            FOObjPool[i].transform.position = new Vector3(Random.Range(FOText.gameObject.transform.position.x - 1, FOText.gameObject.transform.position.x + 1), Random.Range(FOText.gameObject.transform.position.y - 2, FOText.gameObject.transform.position.y), Random.Range(FOText.gameObject.transform.position.z - 1, FOText.gameObject.transform.position.z + 1));
            FOObjPool[i].SetActive(true);
            pos = gun.gameObject.transform.position;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(5);
        upMagazineGun = false;
    }

    public void FireEnd()
    {
        fireOpen = false;
        aimVisual.gameObject.SetActive(false);
        myMove = false;

        QueueController.Instance.NextQueue(true);
    }

    public void ActiveInput()
    {
        if (!StaticGameController.Instance.gameIsPlayed && !myMove) return;

        drag = true;
        Vector2 touchPos = Vector2.zero;
#if UNITY_EDITOR
        touchPos = Mouse.current.position.ReadValue();
#else
        touchPos = Touchscreen.current.position.ReadValue();
#endif
        lastTouchPos = touchPos;

    }

    public void DeactiveInput()
    {
        if (!StaticGameController.Instance.gameIsPlayed && !myMove) return;

        drag = false;
    }

    private Vector2 lastTouchPos;
    Vector3 pos;

    private bool drag;
    private void FixedUpdate()
    {
        if (!StaticGameController.Instance.gameIsPlayed && !myMove) return;

        if (upMagazineGun)
        {
            for (int i = 0; i < FOObjPool.Count; i++)
            {
                if (FOObjPool[i].activeInHierarchy)
                    FOObjPool[i].transform.position = Vector3.Lerp(FOObjPool[i].transform.position, pos, 5 * Time.deltaTime);
                if (Vector3.Distance(FOObjPool[i].transform.position, gun.gameObject.transform.position) < 0.1f)
                {
                    gun.GunAnimUse();
                    FOObjPool[i].SetActive(false);
                }
            }
        }

        if (fireOpen)
        {
            if (drag)
            {
                Vector2 touchPos = Vector2.zero;
#if UNITY_EDITOR
                touchPos = Mouse.current.position.ReadValue();
#else
        touchPos = Touchscreen.current.position.ReadValue();
#endif
                Vector2 newPos = Vector2.zero;

                 if (touchPos.x != lastTouchPos.x)
                 {
                     if (touchPos.x > lastTouchPos.x)
                         newPos.x = (touchPos.x - lastTouchPos.x);
                     else
                         if (touchPos.x < lastTouchPos.x)
                         newPos.x = -(lastTouchPos.x - touchPos.x);
                 }

                 if (touchPos.y != lastTouchPos.y)
                 {
                     if (touchPos.y > lastTouchPos.y)
                         newPos.y = (touchPos.y - lastTouchPos.y);
                     else
                     if (touchPos.y < lastTouchPos.y)
                         newPos.y = -(lastTouchPos.y - touchPos.y);
                 }

                aimVisual.transform.position = new Vector2(aimVisual.transform.position.x + newPos.x * AIMMoveSpeed,
                       aimVisual.transform.position.y + newPos.y * AIMMoveSpeed);

                lastTouchPos = touchPos;
            }

            //aimVisual.rectTransform.transform.position = new Vector2(aimVisual.rectTransform.transform.position.x + JoystickStick.Instance.HorizontalAxis() * AIMMoveSpeed,
            //    aimVisual.rectTransform.transform.position.y + JoystickStick.Instance.VerticalAxis() * AIMMoveSpeed);

            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(aimVisual.rectTransform.position.x, aimVisual.rectTransform.position.y)), out hit);

            fireTarget.position = hit.point;
            GunObj.transform.LookAt(hit.point);
        }
    }
}
