using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using System;
public class Tutorial : MonoBehaviour
{
    public Image cursorGaugeImage; //커서게이지 이미지 변수

    public GameObject character; //자신의 캐릭터 변수
    public GameObject mainCam; //카메라 변수
    public GameObject hintM; //메세지창 오브젝트 변수
    public GameObject[] door= new GameObject[2]; //key 및 문에 사용할 변수 지정

    public AudioSource[] audioSource = new AudioSource[5]; //오디오 클립에 사용할 변수들

    public TextMeshProUGUI hintMessage; //메세지 출력 변수

    private Ray ray; //Ray 변수

    //gaugeTimer 초기화 및 걷는 속도 초기화
    private float gaugeTimer = 0.0f; 
    private float gazeTime = 1.0f;
	private float walkSpeed = 2.0f;
    //isName을 통해 반환값 저장
    private string isName;

    private bool[] confirm = new bool[2]; //양피지 경우의 수 배열 지정.
    private bool message = false; //message 값 초기화
    private bool door_key=false; //키 변수 초기화
    private bool startMessage = false;

	Vector3 dir;
	void Start()
    {
        //함수 시작과 함께 Prefs에 있던 정보들 초기화 및 튜토리얼 메세지, 오디오 출력
        PlayerPrefs.DeleteAll();
        audioSource[1].Play();
        startMessage = true;
        hintM.SetActive(true);
        hintMessage.text = "튜토리얼에 오신 것을 환영합니다."+ "\n"+"방을 둘러보며 튜토리얼을 완료하세요."
            +"\n"+"상단의 버튼 눌러서 메시지를 닫을 수 있습니다.";
        StartCoroutine(StartMessage());
    }

    void Update()
    {
        RaycastHit hit;
        //ray의 길이 및 카메라 위치 조정
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
            //message 창이 켜져 있으면, 클릭해서 비활성화
            message = false;
            hintM.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0)) //마우스 버튼이 눌리면 걷는 소리 재생
            audioSource[0].Play();
        else if (Input.GetMouseButtonUp(0)) //마우스 버튼 떼어지면 소리 정지
            audioSource[0].Stop();

        if (!startMessage && Input.GetMouseButton(0)) //마우스 버튼이 눌릴시 앞으로 가기 함수 활성화
		{
            gaugeTimer = 0.0f;
            MoveForward(true);
            
        }

        else
        {
            MoveForward(false);
            
        }
        if (Physics.Raycast(ray.origin, forward, out hit)) //Tag가 hint인 것만 게이지 차게함
        {
            if ((hit.collider.CompareTag("Hint") || hit.collider.CompareTag("meaningless") || hit.collider.CompareTag("Button")))
            {
                //message가 켜져있으면 게이지 비활성화
                    if (!message)
                    {
                        gaugeTimer += 1.0f / gazeTime * Time.deltaTime;
                    }
                    if (gaugeTimer >= 1.0f)
                    {
                    //게이지가 활성화가 되면 이름 저장 및 메세지 출력
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

    }
    void MoveForward(bool isMoving)
	{
        //앞으로 나가는 함수 구현
        if (isMoving)
        {
            //앞으로 가는 모션 및 식 구현
            character.gameObject.GetComponent<Animator>().SetBool("isMoving", true);
            character.gameObject.GetComponent<Animator>().SetBool("Idle", false);
            gameObject.transform.Translate(dir.x * walkSpeed * Time.deltaTime, 0, dir.z * walkSpeed * Time.deltaTime);
            
        }
        else
        {
            //멈춤 모션 구현
            character.gameObject.GetComponent<Animator>().SetBool("isMoving", false);
            character.gameObject.GetComponent<Animator>().SetBool("Idle", true);

        }

    }
	

    void Message(RaycastHit hit)
    {
        //오디오 소스를 틀고, 각 상황에 맞는 메세지들 출력
        audioSource[2].Play();
        if (hit.collider.CompareTag("meaningless"))
        {
            hintM.SetActive(true);
            hintMessage.text = "별 다른 특징이 없는 물건은"+"\n"+"'특별해 보이지 않는다.'"+"\n"+"라는 메세지가 출력됩니다.";
        }
        if (hit.collider.name.Equals("tutorial_book")) //책상 종이 힌트
        {
            hintM.SetActive(true);
            hintMessage.text = "책장 및 방을 살펴보며 튜토리얼을 시행하세요.";
        }
        if(!confirm[0] &&hit.collider.name.Equals("Scroll_Stack (1)"))
        {
            confirm[1] = true;
            hintM.SetActive(true);
            hintMessage.text = "잘하셨습니다!" + "\n" +"해당 물건보다 아래있는 양피지를 바라본 후"+"\n"+"상호작용을 시작하십시오.";

        }
        else if((confirm[0]) && hit.collider.name.Equals("Scroll_Stack (1)")) //각 조건 달성시 키 활성화 시킴
        {
            door[1].SetActive(true);
            hintM.SetActive(true);
            hintMessage.text = "식탁 위에 열쇠가 생성되었습니다."+"\n"+"상호작용을 한 후 튜토리얼을 완료하십시오.";
        }
        if (!confirm[1] && hit.collider.name.Equals("Scroll_Stack (2)"))
        {
            confirm[0] = true;
            hintM.SetActive(true);
            hintMessage.text = "잘하셨습니다!"+"\n"+"해당 물건보다 위에있는 양피지를 바라본 후" + "\n" + "상호작용을 시작하십시오.";
        }
        else if((confirm[1]) && hit.collider.name.Equals("Scroll_Stack (2)")) //각 조건 달성시 키 활성화 시킴
        {
            door[1].SetActive(true);
            hintM.SetActive(true);
            hintMessage.text = "식탁 위에 열쇠가 생성되었습니다." + "\n" + "상호작용을 한 후 튜토리얼을 완료하십시오.";
        }
        if (hit.collider.name.Equals("Key_Rusty (1)"))
        {
            door_key = true;
            hintM.SetActive(true);
            hintMessage.text = "열쇠를 획득했습니다."+"\n"+"뒤에 있는 문과 상호작용을 한 후 방을 탈출하십시오.";
            door[1].SetActive(false);
        }
        if (door_key && (hit.collider.name.Equals("Door2"))) //키가 있을경우 오디오 소스와 함께 애니메이션 구현
        {
            audioSource[3].Play();
            door[0].gameObject.GetComponent<Animator>().SetBool("Open", true);
            hintM.SetActive(true);
            hintMessage.text = "문이 열렸다.";
        }
        else if(!door_key && (hit.collider.name.Equals("Door2"))) //없을시 다음과 같은 오디오소스와 함께 메세지 출력
        {
            audioSource[4].Play();
            hintM.SetActive(true);
            hintMessage.text = "문이 굳게 잠겨 있다.";
        }
    }
    IEnumerator StartMessage()
    {
        yield return new WaitForSeconds(2.0f);
        startMessage = false;
        message = true;
    }
}
