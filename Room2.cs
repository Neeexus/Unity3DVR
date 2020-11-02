using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room2 : MonoBehaviour
{
	public GameObject character;
    public Image cursorGaugeImage;
    public GameObject mainCam;
    public GameObject hintM;
    public GameObject box;
    public GameObject[] door= new GameObject[2];
    public GameObject[] items = new GameObject[4];
    public TextMeshProUGUI hintMessage;
    private Ray ray;
    public AudioSource[] audioSource = new AudioSource[5];

    private float gaugeTimer = 0.0f;
    private float gazeTime = 1.0f;
	private float walkSpeed = 2.0f;

    private bool isMoving = false;
    private bool[] itemFound = new bool[4];
    private bool foundclue;
    private bool door_key = false;
    private bool box_key = false;
    private bool message = false;
    private bool startMessage = false;

    private string isName;
    


	Vector3 dir;
	// Start is called before the first frame update
	void Start()
    {
        startMessage = true;
        ray = new Ray();
        audioSource[1].Play();
        

        hintM.SetActive(true);
        hintMessage.text = "조는 주인공이 도망가지 못하도록 \n 투명한 유리방에 가두었습니다." +
            " \n 유리방 속에서 단서를 찾고, \n 조와의 싸움에 필요한 아이템을 찾으세요!";
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
        if (message == true  && Input.GetMouseButton(0)) //
        {
            message = false;
            isMoving = false;
            hintM.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
            audioSource[0].Play();
        else if (Input.GetMouseButtonUp(0))
            audioSource[0].Stop();

        if (!startMessage && Input.GetMouseButton(0))
		{
			isMoving = true;
            character.gameObject.GetComponent<Animator>().SetBool("isMoving", true);
            character.gameObject.GetComponent<Animator>().SetBool("Idle", false);
            MoveForward();
		}
        else
        {
            isMoving = false;
            character.gameObject.GetComponent<Animator>().SetBool("isMoving", false);
            character.gameObject.GetComponent<Animator>().SetBool("Idle", true);
        }
			


        if (Physics.Raycast(ray.origin, forward, out hit) && (hit.collider.CompareTag("Hint") || hit.collider.CompareTag("meaningless"))) //Tag가 hint인 것만 게이지 차게함
		{

			if (!isMoving)        // 멈춰있는 상태에서 게이지 차도록 함 + 현재 오브젝트의 이름과 일치하지않을경우에만 게이지 차도록 함
			{
                if(!message)
				    gaugeTimer += 1.0f / gazeTime * Time.deltaTime;

				if (gaugeTimer >= 1.0f)
				{
                    
                    OnTriggerEnter(hit.collider);
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
			gaugeTimer = 0.0f;

    }
	void MoveForward()
	{
		if(isMoving)
			// 앞으로 이동
			gameObject.transform.Translate(dir.x * walkSpeed * Time.deltaTime, 0, dir.z * walkSpeed * Time.deltaTime);
    }
	
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.name.Equals(isName))    //other의 태그불러오기
			isMoving = false;

	}
    void Message(RaycastHit hit)
    {
        audioSource[4].Play();

        if (hit.collider.CompareTag("meaningless"))
        {
            hintM.SetActive(true);
            hintMessage.text = "특별해 보이지는 않는다.";
        }
        if (hit.collider.name.Equals("Stage1_book1"))
        {
            hintM.SetActive(true);
            hintMessage.text = "'추울까봐 난로 틀어놨어.' 라고 쓰여져 있다.";
        }
        
        if (hit.collider.name.Equals("Stage1_paper1"))
        {
            foundclue = true;
            hintM.SetActive(true);
            hintMessage.text = "'죽음은 영원한 궁정의 문을 여는 황금 열쇠이다." + "\n" + "(죤 밀턴)'" + "\n" + "라고 쓰여져 있다."+"\n"+"죽음과 관련된 물건을 찾아보자.";
                
        }
        if (hit.collider.name.Equals("Stage1_skull"))
        {
            if (!foundclue)
            {
                hintM.SetActive(true);
                hintMessage.text = "수상하게 생긴 해골이다.";
            }
            else
            {
                hit.transform.gameObject.SetActive(false);
                hintM.SetActive(true);
                hintMessage.text = "해골을 치워보니 물건이 흐릿하게 보인다.";
            }
            
        }
               
        if (hit.collider.name.Equals("Stage1_key1"))
        {
            box_key = true;
            hintM.SetActive(true);
            hintMessage.text = "열쇠를 획득 했다.";
            hit.transform.gameObject.SetActive(false);
        }

        if (hit.collider.name.Equals("Stage1_key2"))
        {
            door_key = true;
            hintM.SetActive(true);
            hintMessage.text = "열쇠를 획득 했다.";
            hit.transform.gameObject.SetActive(false);
        }

        if (!box_key && hit.collider.name.Equals("Stage1_Box1"))
        {
            audioSource[3].Play();
            hintM.SetActive(true);
            hintMessage.text = "상자가 굳게 잠겨있다.";
        }
        else if (box_key && hit.collider.name.Equals("Stage1_Box1"))
        {
            audioSource[2].Play();
            hit.transform.gameObject.SetActive(false);
            box.SetActive(true);
            box_key = false;

        }
        if (hit.collider.name.Equals("Stage1_paper2"))
        {
            hintM.SetActive(true);
            hintMessage.text = "'나는 벤치에 앉아서 책 읽기를 좋아해.'"+"\n"+"라고 쓰여져 있다.";
        }

        if (hit.collider.name.Equals("Stage1_paper3"))
        {
            hintM.SetActive(true);
            hintMessage.text = "'등잔 밑이 어둡다'"+"\n"+"라고 쓰여져 있다.";
        }
        if (hit.collider.name.Equals("Stage1_key3"))
        {
            door_key = true;
            hintM.SetActive(true);
            hintMessage.text = "열쇠를 획득 했다.";
            hit.transform.gameObject.SetActive(false);
        }
        if (hit.collider.name.Equals("Shield"))
        {
            itemFound[3] = true;
            hintM.SetActive(true);
            hintMessage.text = "방패를 획득 했다."+"\n"+"조와 싸울 때 유용할 것 같다.";
            hit.transform.gameObject.SetActive(false);
        }
        if (hit.collider.name.Equals("Sword"))
        {
            itemFound[0] = true;
            hintM.SetActive(true);
            hintMessage.text = "검을 획득 했다." + "\n" + "조와 싸울 때 유용할 것 같다.";
            hit.transform.gameObject.SetActive(false);
        }
        if (hit.collider.name.Equals("Potion_Red"))
        {
            itemFound[1] = true;
            hintM.SetActive(true);
            hintMessage.text = "포션같은 것을 획득 했다." + "\n" + "조와 싸울 때 유용할 것 같다.";
            hit.transform.gameObject.SetActive(false);
        }
        if (hit.collider.name.Equals("Potion_Blue"))
        {
            itemFound[2] = true;
            hintM.SetActive(true);
            hintMessage.text = "마셔보니 힘이 강해진것 같다.." + "\n" + "조와 싸울 때 유용할 것 같다.";
            hit.transform.gameObject.SetActive(false);
        }

        if (!door_key && (hit.collider.name.Equals("Door2")))
        {
            audioSource[3].Play();
            if (!itemFound[0] || !itemFound[1] || !itemFound[2] || !itemFound[3])
            {
                hintM.SetActive(true);
                hintMessage.text = "아직 찾을 물건이 몇가지 남은 것 같다.";
            }
            else if((itemFound[0] && itemFound[1] && itemFound[2] && itemFound[3]))
            {
                hintM.SetActive(true);
                hintMessage.text = "물건들은 찾았지만 열쇠를 아직 못찾았다.";
            }
        }
        else if(door_key && (hit.collider.name.Equals("Door2")&& (!itemFound[0] || !itemFound[1] || !itemFound[2] || !itemFound[3])))
        {
            hintM.SetActive(true);
            hintMessage.text = "아직 찾을 물건이 몇가지 남은 것 같다.";
        }

        if (!door_key && (hit.collider.name.Equals("Door")))
        {
            audioSource[3].Play();
            hintM.SetActive(true);
            hintMessage.text = "문이 굳게 잠겨있다.";
        }
        if (door_key && (hit.collider.name.Equals("Door")))
        {
            audioSource[2].Play();
            door[0].gameObject.GetComponent<Animator>().SetBool("Open", true);
            door_key = false;
            hintM.SetActive(true);
            hintMessage.text = "문이 열렸다."+"\n"+ "조와 싸우기 위해서는"+"\n"+"도움이 될 물건을"+"\n"+ "찾아 나가야 될 것 같다.";
        }
        if ((itemFound[0] && itemFound[1]&& itemFound[2]&& itemFound[3]) && door_key && (hit.collider.name.Equals("Door2")))
        {
            PlayerPrefs.SetInt("item", 1);
            audioSource[2].Play();
            door[1].gameObject.GetComponent<Animator>().SetBool("Open", true);
            door_key = false;
            hintM.SetActive(true);
            hintMessage.text = "문이 열렸다.";

        }
    }
    IEnumerator StartMessage()
    {
        yield return new WaitForSeconds(2.0f);
        startMessage = false;
        message = true;
    }
}
