using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room0 : MonoBehaviour
{
	public GameObject character;
    public Image cursorGaugeImage;
    public GameObject mainCam;
    public GameObject hintM;
    public GameObject box;
    public GameObject[] door= new GameObject[2];
    
    public AudioSource[] audioSource= new AudioSource[5];

    public TextMeshProUGUI hintMessage;

    public bool[] hintKey = new bool[5];

    private float gaugeTimer = 0.0f;
    private float gazeTime = 1.0f;
	private float walkSpeed = 2.0f;
    
    private string isName;
    
    private bool message = false;
    private bool startMessage = false;
    private Ray ray;
    
    Vector3 dir;
	
    // Start is called before the first frame update
	void Start()
    {
        startMessage = true;
        ray = new Ray();
        audioSource[1].Play();
        hintM.SetActive(true);
        hintMessage.text = "눈을 떠보니 알 수 없는 곳에 갇히게 되었다. \n " +
            "방안에 숨겨져 있는 단서를 최대한 활용하여 \n 방에서 탈출해야 합니다.";
        StartCoroutine(StartMessage());
    }
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 forward = mainCam.transform.TransformDirection(Vector3.forward) * 1000;

        mainCam.transform.localPosition = new Vector3(character.transform.localPosition.x, character.transform.localPosition.y + 1.62f, character.transform.localPosition.z);
        dir = mainCam.transform.localRotation * Vector3.forward;
        cursorGaugeImage.fillAmount = gaugeTimer;
        // 캐릭터를 카메라가 바라보는 방향으로 회전
        character.gameObject.transform.localRotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        //카메라의 위치를 캐릭터의 위치와 일치
        ray.origin = mainCam.transform.position;
        Debug.DrawRay(ray.origin, forward, Color.green);
        if (message == true  && Input.GetMouseButtonDown(0)) //
        {
            message = false;
            
            hintM.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            audioSource[0].Play();
        }
        else if (Input.GetMouseButtonUp(0))
            audioSource[0].Stop();


        if (!startMessage && Input.GetMouseButton(0))
		{
             MoveForward(true);
            gaugeTimer = 0.0f;
        }
        else
        {
            MoveForward(false);
        }

			
        if (Physics.Raycast(ray.origin, forward, out hit) && (hit.collider.CompareTag("Hint") || hit.collider.CompareTag("meaningless"))) //Tag가 hint인 것만 게이지 차게함
		{
            
            
            if (!hit.transform.name.Equals(isName))        // 멈춰있는 상태에서 게이지 차도록 함 + 현재 오브젝트의 이름과 일치하지않을경우에만 게이지 차도록 함
			{
                if (!message)
                {
                    gaugeTimer += 1.0f / gazeTime * Time.deltaTime;

                }
				if (gaugeTimer >= 1.0f)
				{

					isName = hit.collider.gameObject.name; //현재 콜리더의 이름을 isName으로 할당
                    Message(hit);
                    
                    message = true;
                    gaugeTimer = 0.0f;
                }
            }			
			else
			{
				gaugeTimer = 0.0f;
            }
        }
        else
        {
            gaugeTimer = 0.0f;
        }


    }
	void MoveForward(bool isMoving)
	{

        if (isMoving)
        {
            character.gameObject.GetComponent<Animator>().SetBool("isMoving", true);
            character.gameObject.GetComponent<Animator>().SetBool("Idle", false);
            gameObject.transform.Translate(dir.x * walkSpeed * Time.deltaTime, 0, dir.z * walkSpeed * Time.deltaTime);
            
            
        }
        else
        {
            
            character.gameObject.GetComponent<Animator>().SetBool("isMoving", false);
            character.gameObject.GetComponent<Animator>().SetBool("Idle", true);

        }

    }
	

    void Message(RaycastHit hit)
    {

        audioSource[4].Play();
        if (hit.collider.CompareTag("meaningless"))
        {
            hintM.SetActive(true);
            hintMessage.text = "특별해보이지는 않는다.";
        }
        if (hit.collider.name.Equals("Stage0_Paper1")) //책상 종이 힌트
        {
            hintKey[0] = true;
            hintM.SetActive(true);
            hintMessage.text = "'나는 어릴 적부터 창밖을 보는걸 좋아했어'"+"\n"+ "라고 써져있다. \n 창 밖을 둘러보자.";
        }
        if (hit.collider.name.Equals("Stage0_Key1")) //창 밖키 획득
        {
            if (hintKey[0])
            {
                hintKey[1] = true; //창문키를 획득
                hintM.SetActive(true);
                hintMessage.text = "열쇠 같은 것을 얻었다."+"\n"+"이상하게 생겼지만"+"\n"+"서랍 같은 곳에 들어갈 것 같다.";
                hit.transform.gameObject.SetActive(false);
            }
            else
            {
                hintM.SetActive(true);
                hintMessage.text = "뿌리 같이 생겼다."+"\n"+"어디에 쓰이는지는 잘 모르겠다.";
            }

        }
        if (hit.collider.name.Equals("Stage0_Key2")) //서랍열쇠 상호작용
        {
            hintKey[2] = true; //장롱키 획득
            hintM.SetActive(true);
            hintMessage.text = "열쇠 하나를 얻었다. \n 가구 같은 곳에 들어갈 것 같다.";
            hit.transform.gameObject.SetActive(false);
        }
        if (hit.collider.name.Equals("Stage0_Key3")) //장롱열쇠 상호작용
        {
            hintKey[3] = true; //상자키
            hintM.SetActive(true);
            hintMessage.text = "다른 열쇠 하나를 더 얻었다. \n 크기가 상자에 맞을 것 같다.";
            hit.transform.gameObject.SetActive(false);
        }
        if (hit.collider.name.Equals("Stage0_Key4")) //상자열쇠 상호작용
        {
            hintKey[4] = true; //문 키
            hintM.SetActive(true);
            hintMessage.text = "열쇠를 얻었다. \n 문에 꼭 들어 맞게 생겼다.";
            hit.transform.gameObject.SetActive(false);
        }


       
        if (!hintKey[1] && hit.collider.name.Equals("Stage0_desk"))
        {
            audioSource[3].Play();
            hintM.SetActive(true);
            hintMessage.text = "서랍이 잠겨있다.";
        }
        else if(hintKey[1] && hit.collider.name.Equals("Stage0_desk"))
        {
            audioSource[2].Play();
            door[2].gameObject.GetComponent<Animator>().SetBool("Open", true);
        }


        if (hintKey[2] && ((hit.collider.name.Equals("Closet_R")) || (hit.collider.name.Equals("Closet_L"))))
        {
            audioSource[2].Play();
            door[0].gameObject.GetComponent<Animator>().SetBool("Open", true);
            door[1].gameObject.GetComponent<Animator>().SetBool("Open", true);
        }
        else if (!hintKey[2] && ((hit.collider.name.Equals("Closet_R")) || (hit.collider.name.Equals("Closet_L"))))
        {
            audioSource[3].Play();
            hintM.SetActive(true);
            hintMessage.text = "장롱이 잠겨있다.";
        }
        if (hintKey[3] && (hit.collider.name.Equals("Stage0_Box1")))
        {
            audioSource[2].Play();
            hit.transform.gameObject.SetActive(false);
            box.SetActive(true);
        }
        else if (!hintKey[3] && (hit.collider.name.Equals("Stage0_Box1")))
        {
            audioSource[3].Play();
            hintM.SetActive(true);
            hintMessage.text = "상자가 잠겨있다.";
        }
        if (!hintKey[4] && hit.collider.name.Equals("Stage0_Exit0"))
        {
            audioSource[3].Play();
            hintM.SetActive(true);
            hintMessage.text = "문이 굳게 잠겨있다.";
        }
        else if (hintKey[4] && (hit.collider.name.Equals("Stage0_Exit0")))
        {
            audioSource[2].Play();
            door[3].gameObject.GetComponent<Animator>().SetBool("Open", true);
        }

    }
    IEnumerator StartMessage()
    {
        yield return new WaitForSeconds(2.0f);
        startMessage = false;
        message = true;
    }
}
