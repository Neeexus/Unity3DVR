using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
public class Room1 : MonoBehaviour
{
    public Image cursorGaugeImage; //커서게이지 이미지 지정

    public GameObject character; 
    public GameObject mainCam;
    public GameObject hintM;
    public GameObject box;
    public GameObject[] door= new GameObject[2];
    public GameObject ui;
    public GameObject man;

    public AudioSource[] audioSource = new AudioSource[6];
    
    public TextMeshProUGUI hintMessage;
    public TextMeshProUGUI keyPadMessage;
    public TextMeshProUGUI endCredit;

    public bool[] clue = new bool[4];
    private bool message = false;
    private bool startMessage = false;

    private Ray ray;
    private float gaugeTimer = 0.0f;
    private float gazeTime = 1.0f;
	private float walkSpeed = 2.0f;
    
    private string password_input;
    private string password;
    private string isName;
    
    private int win;

    Vector3 dir;
	// Start is called before the first frame update
	void Start()
    {
        audioSource[1].Play();
        win = PlayerPrefs.GetInt("win");
        if (win == 1)
        {
            startMessage = true;
            man.SetActive(false);
            hintM.SetActive(true);
            hintMessage.text = "조를 죽였다.. 방에서 나갈 방법을 찾아보자.";
            StartCoroutine(StartMessage());
        }
        else
        {
            startMessage = true;
            man.SetActive(true);
            hintM.SetActive(true);
            hintMessage.text = "이곳은 조의 서재인 것 같다. \n 조를 피해서 탈출 해보자";
            StartCoroutine(StartMessage());
        }
            
        ray = new Ray();
        password = "3956"; //9+6+3011+930
       
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
        if (message == true  && Input.GetMouseButton(0))
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

        if (!startMessage &&Input.GetMouseButton(0))
		{
            MoveForward(true);
            gaugeTimer = 0.0f;
        }

        else
        {
            MoveForward(false);
            
        }

        if (Physics.Raycast(ray.origin, forward, out hit)) //Tag가 hint인 것만 게이지 차게함
        {
                if ((hit.collider.CompareTag("Hint") || hit.collider.CompareTag("meaningless") || hit.collider.CompareTag("Button")))
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
            }
            else
            {
                gaugeTimer = 0.0f;
            }
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
        audioSource[5].Play();
        if (hit.collider.name.Equals("smartphone"))  //엔딩
        {


            audioSource[1].Stop();
            audioSource[2].Play();
            hintM.SetActive(true);
            hintMessage.text ="<Game End>"+ "\n"+"휴대폰을 발견하여 구조대에 "+"\n"+ "신고하였고, 탈출에 성공하였다..!";
            StartCoroutine(endingCredit());
            endCredit.gameObject.SetActive(true);
        }
        if (hit.collider.CompareTag("meaningless"))
        {
            hintM.SetActive(true);
            hintMessage.text = "특별해보이지는 않는다...";
        }

        if ((!clue[0] || !clue[1])&&hit.collider.name.Equals("Stage2_Laptop"))
        {
                
                hintM.SetActive(true);
                hintMessage.text = "컴퓨터가 켜졌지만"+"\n"+"패스워드가 필요하다."+"\n"+"힌트: 멈춘 시간 속 늑대" ;
                hit.transform.GetComponent<MeshRenderer>().material = Resources.Load("Laptop_white", typeof(Material)) as Material;
        }
        else if((clue[0] && clue[1])&& (hit.collider.name.Equals("Stage2_Laptop"))){
            hintM.SetActive(true);
            hintMessage.text = "로그인이 되었다." + "\n" + "화면에는" + "\n" + " <0001>+<0010>" + "\n" + "+<0011>+<0100>" + "\n" + "라고 나와있다.";

            hit.transform.GetComponent<MeshRenderer>().material = Resources.Load("Laptop_white", typeof(Material)) as Material;
        }

        if (hit.collider.name.Equals("Stage2_Keypad")) //키패드 호출
        {
            ui.SetActive(true);

        }

        if (hit.collider.CompareTag("Button"))
        {

            if (hit.collider.name.Equals("ButtonGreen"))//입력버튼
            {
                if (password == password_input) // password일치할떄
                {
                    ui.SetActive(false);
                    hintM.SetActive(true);
                    hintMessage.text = "문이 열렸다..!";
                    password_input = null;
                    keyPadMessage.text = password_input;
                    door[0].gameObject.GetComponent<Animator>().SetBool("Open", true);
                    
                    audioSource[3].Play();
                    


                }
                else if (password != password_input)  // password 틀렸을때
                {
                    ui.SetActive(false);
                    hintM.SetActive(true);
                    hintMessage.text = "비밀번호가 틀리다!";
                    password_input = null;
                    keyPadMessage.text = password_input;
                }
                

            }
            else if (hit.collider.name.Equals("ButtonRed"))
            {
                password_input = null;
                keyPadMessage.text = password_input;
            }
            else
            {
                password_input += isName;
                keyPadMessage.text = password_input;
                
            }
        }



        if (hit.collider.name.Equals("Door1"))
        {
            audioSource[4].Play();
            hintM.SetActive(true);
            hintMessage.text = "문이 잠겨있다.";
        }









        if (hit.collider.name.Equals("Stage2_Book1"))
        {
            hintM.SetActive(true);
            hintMessage.text = "<0001>"+"\n" + "7 + 7 = 2" + "\n" + "8 + 8 = 4 " + "\n" + "6 + 9 = 3" + "\n" + "10 + 11 = 9" + "\n" + "then, 12 + 9 = ?"  ;
            //답 9
        }



        if (hit.collider.name.Equals("Stage2_Paper1"))
        {
            hintM.SetActive(true);
            hintMessage.text = "<0010>" + "\n" + "9 = 72" + "\n" + "8 = 56 " + "\n" + "7 = 42 " + "\n" + "6 = 30 " + "\n" + "5 = 20 " + "\n" + "then, 3 = ? ";
            //답 6
        }

        if (hit.collider.name.Equals("Stage2_Book2"))
        {
            hintM.SetActive(true);
            hintMessage.text = "<0011>" + "\n" + "1, 2 = 23" + "\n" + "2, 3  = 65 " + "\n" + "3, 4 = 127" + "\n" + "4, 5 = 209" + "\n" + "then, 5, 6 = ?";
            //답 3011
        }

        if (hit.collider.name.Equals("Stage2_Notehint1"))
        {
            clue[0] = true;
            hintM.SetActive(true);
            hintMessage.text = "I wolf you..?"+"\n"+"컴퓨터 패스워드와 관련이"+"\n"+"있어보인다.";
        }
        if (hit.collider.name.Equals("Stage2_Notehint2"))
        {
            clue[1] = true;
            hintM.SetActive(true);
            hintMessage.text = "<0100>"+"\n"+"시계가 멈춰있다."+"\n"+"9:30을 가리키고 있다."+"\n"+ "컴퓨터 패스워드와 관련이"+"\n"+"있어보인다.";
        }
        if (hit.collider.name.Equals("Coffin_1_2")|| hit.collider.name.Equals("Coffin_1_2 (3)") || hit.collider.name.Equals("Coffin_1_2 (2)") || hit.collider.name.Equals("Coffin_1_2 (1)"))
        {
            hintM.SetActive(true);
            hintMessage.text = "출구가 막혀있다."+"\n"+"핸드폰을 통해 구조를 요청해보자.";
        }
       
    }
    IEnumerator endingCredit()
    {
        yield return new WaitForSeconds(35.0f);
        SceneManager.LoadScene("Main_Menu");
    }
    IEnumerator StartMessage()
    {
        yield return new WaitForSeconds(2.0f);
        startMessage = false;
        message = true;
    }
}
