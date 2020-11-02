using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Room3 : MonoBehaviour
{
    
    public Image cursorGaugeImage;
    public Image HPBar;
    public Image E_HPbar;
    

    public GameObject mainCam;
    public GameObject hintM;
    public GameObject inventory;

    public GameObject[] item = new GameObject[4];
    public GameObject[] Spawn_Point = new GameObject[3];
    public GameObject[] Character = new GameObject[2];
    public TextMeshProUGUI[] info = new TextMeshProUGUI[6];
    public AudioSource[] audioSource = new AudioSource[4];
    
    
    public TextMeshProUGUI hintMessage;
    public TextMeshProUGUI explain;
    public TextMeshProUGUI fightInfoM;

    private Ray ray;
 
    private float gaugeTimer = 0.0f;

    private string isName;
    private string infoName;
    private string getFightInfo;

    bool message = false;
    bool selected = false;
    bool playOnce = false;

    private int HP;
    private int AP;
    private int block;
    private int itemIdx;

    private bool dead = false;
    private bool E_Dead = false;
    private bool myturn = false;
    private bool E_turn = false;

    private int EnemyHP;
    private int EnemyAP;

    private int AP_Potion;
    private int HP_Potion;

    Vector3 dir;
    void Start()
    {
        getFightInfo = "당신은 조와 마주쳤습니다! \n 캐릭터 하단의 아이콘을 '클릭' 하여 \n 싸우십시오.";
        audioSource[0].Play();
        itemIdx = PlayerPrefs.GetInt("item");
        if (itemIdx == 1)
        {
            AP = 40;
            block = 2;
            HP_Potion = 30;
            AP_Potion = 10;
        }
        
        else
        {
            item[0].SetActive(false);
            item[3].SetActive(false);
            HP_Potion = 0;
            AP_Potion = 0;
            AP = 10;
            block = 1;
        }
        myturn = true;
        ray = new Ray();
        HP = 100;
        
        EnemyHP = 200;
        EnemyAP = 40;
        StartCoroutine(MessageAppend());

    }

    // Update is called once per frame
    void Update()
    {

       

        RaycastHit hit;
        Vector3 forward = mainCam.transform.TransformDirection(Vector3.forward) * 1000;
        cursorGaugeImage.fillAmount = gaugeTimer;
        HPBar.fillAmount = HP * 0.01f;
        E_HPbar.fillAmount = EnemyHP * 0.005f;
        // 캐릭터를 카메라가 바라보는 방향으로 회전
        //카메라의 위치를 캐릭터의 위치와 일치
        ray.origin = mainCam.transform.position;

        info[0].text = AP.ToString();
        info[1].text = EnemyAP.ToString()+"/"+block;
        info[2].text = "+"+HP_Potion.ToString();
        info[3].text = "+" + AP_Potion.ToString();
        info[4].text = "체력: "+ HP.ToString();
        info[5].text = "체력: "+ EnemyHP.ToString();


        Debug.DrawRay(ray.origin, forward, Color.green);



        if (Physics.Raycast(ray.origin, forward, out hit) && (hit.collider.CompareTag("Button"))) //Tag가 Button인 것만 게이지 차게함
        {

            infoName = hit.collider.gameObject.name;
            if (myturn)
            {
                if (Input.GetMouseButtonDown(0))
                {

                    selected = true;
                    isName = hit.collider.gameObject.name; //현재 콜리더의 이름을 isName으로 할당
                    myturn = true;

                }
            }

            if (infoName == "Attack")
            {
                explain.gameObject.SetActive(true);
                explain.text= "현재 공격력" +"\n"+ "상호작용을 할 시 공격합니다.";
            }
            else if (infoName == "Deffend")
            {
                explain.gameObject.SetActive(true);
                explain.text = "현재 방어력" + "\n" + "상호작용을 할 시 적의 공격력을"+"\n"+"영구적으로 적 공격력 /"+block+"시킵니다."+"\n"+"단, 적의 최소 공격력은 10입니다.";
            }
            else if (infoName == "HP")
            {
                explain.gameObject.SetActive(true);
                explain.text = "현재 캐릭터의 HP +"+HP_Potion+" 만큼 회복시켜줍니다."+"\n"+"단, 플레이어의 HP가 100을 초과할 순 없습니다.";
            }
            else if (infoName == "AP")
            {
                explain.gameObject.SetActive(true);
                explain.text = "현재 캐릭터의 공격력을"+AP_Potion+" 만큼 영구적으로 \n 강화해줍니다."+"\n"+"단, 플레이어의 AP 최대치는 60입니다.";
            }
        }
        else
        {
            explain.gameObject.SetActive(false);

        }
            

        if (HP <= 0)
        {
            getFightInfo = "당신은 패배했습니다.";
            dead = true;
            myturn = false;
            StartCoroutine(MessageAppend());
            StartCoroutine(Player_Dead());


        }
        if (EnemyHP <= 0)
        {
            getFightInfo = "당신이 승리했습니다!";
            E_Dead = true;
            E_turn = false;
            StartCoroutine(MessageAppend());
            StartCoroutine(Enemy_Dead());
        }

        if (!dead && !myturn)
        {
            gaugeTimer = 0.0f;

        }
        else if (selected && myturn)
        {
            inventory.SetActive(false);
            if (hit.transform.name.Equals("HP"))
            {
                
                StartCoroutine(Drink());
                getFightInfo = HP_Potion + "만큼 회복하였습니다.";
                if (itemIdx == 1)
                {
                    
                    item[3].SetActive(false);
                    item[1].SetActive(true);
                }
                else
                    item[1].SetActive(false);

                Message(hit);
                StartCoroutine(MessageAppend());


            }
            else if (hit.transform.name.Equals("AP"))
            {
                
                StartCoroutine(Drink());
                getFightInfo = AP_Potion + "만큼 강화되었습니다.";
                if (itemIdx == 1)
                {
                    item[3].SetActive(false);
                    item[2].SetActive(true);
                }
                else
                    item[2].SetActive(false);

                Message(hit);
                StartCoroutine(MessageAppend());
            }
            else if (hit.transform.name.Equals("Deffend"))
            {
                if (itemIdx == 1)
                {
                    item[3].SetActive(true);
                }
                else
                    item[3].SetActive(false);

                StartCoroutine(Block());
                Message(hit);
                getFightInfo = "적으로 부터 받는피해가 " + EnemyAP + "으로 감소했습니다.";
                StartCoroutine(MessageAppend());
            }
            else if (hit.transform.name.Equals("Attack"))
            {
                
                Message(hit);
                StartCoroutine(Player_Attack());
                StartCoroutine(Enemy_React());
                getFightInfo = "적에게 " + AP + " 만큼의 데미지를 주었습니다.";
                StartCoroutine(MessageAppend());
            }

            selected = false;
        }
        if (!E_Dead && E_turn)
        {
            StartCoroutine(Enemy_Attack());
            StartCoroutine(Player_React());
            
            E_turn = false;
            
        }
        PlayerPrefs.Save();
    }
    void Message(RaycastHit hit)
    {
        audioSource[1].Play();
        if (hit.collider.name.Equals("HP"))
        {
            HP += HP_Potion;
            if (HP >= 100)
            {
                HP = 100;
            }
        }
        if (hit.collider.name.Equals("AP"))
        {
            if (AP >= 60)
            {
                AP = 60;
            }
            else
                AP += AP_Potion;
        }
        if (hit.collider.name.Equals("Attack"))
        {
            Debug.Log("EnemyHP: " + EnemyHP);
        }
        if (hit.collider.name.Equals("Deffend"))
        {
            if (EnemyAP <=10)
            {
                EnemyAP = 10;
            }
            else
                EnemyAP = EnemyAP / block;
        }
    }
    IEnumerator Enemy_Attack()
    {
        yield return new WaitForSeconds(2.0f);
        Character[1].transform.position = Spawn_Point[0].transform.position;
        audioSource[2].Play();
        Character[1].gameObject.GetComponent<Animator>().SetTrigger("Attack");


        yield return new WaitForSeconds(3.15f);




        HP -= EnemyAP;

        Character[1].transform.position = Spawn_Point[2].transform.position;
        Character[1].gameObject.GetComponent<Animator>().SetTrigger("Breath");
        myturn = true;
        E_turn = false;
    }
    IEnumerator Player_Attack()
    {
        Character[0].transform.position = Spawn_Point[0].transform.position;
        audioSource[2].Play();
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("Attack");

        yield return new WaitForSeconds(3.15f);
        EnemyHP -= AP;
        Character[0].transform.position = Spawn_Point[1].transform.position;
        inventory.SetActive(true);
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("Breath");
        E_turn = true;
        myturn = false;
        if (!playOnce && EnemyHP <= 50)
        {
            EnemyAP += 40;
            playOnce = true;
            getFightInfo = "적의 HP가 50이하로 떨어지면 \n 적의 공격력이 40 증가합니다! \n 현재 적의 공격력: " + EnemyAP;
            StartCoroutine(MessageAppend());
        }
    }

    IEnumerator Player_Dead()
    {
        audioSource[3].Play();
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("Dead");
        hintM.SetActive(true);
       
        hintMessage.text = "Game Over";
        
        yield return new WaitForSeconds(4.15f);
        PlayerPrefs.SetInt("win", 2);
        SceneManager.LoadScene("Room2");

    }


    IEnumerator Enemy_Dead()
    {
        audioSource[3].Play();
        yield return new WaitForSeconds(1.15f);
        Character[1].gameObject.GetComponent<Animator>().SetTrigger("Dead");
        myturn = false;
        hintM.SetActive(true);
        hintMessage.text = "You Win!";
        yield return new WaitForSeconds(4.15f);
        PlayerPrefs.SetInt("win", 1);
        SceneManager.LoadScene("Room1");
    }
    IEnumerator Player_React()
    {
        yield return new WaitForSeconds(4.15f);
        getFightInfo = "적에게 " + EnemyAP + " 만큼의 데미지를 받았습니다.";
        StartCoroutine(MessageAppend());
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("React");
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("Idle");
        
    }
    IEnumerator Enemy_React()
    {
        yield return new WaitForSeconds(2.15f);
        Character[1].gameObject.GetComponent<Animator>().SetTrigger("React");
        Character[1].gameObject.GetComponent<Animator>().SetTrigger("Idle");


    }
    IEnumerator Drink() 
    { 
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("Drink");
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("D2I");
        yield return new WaitForSeconds(2.15f);
        inventory.SetActive(true);
        item[1].SetActive(false);
        item[2].SetActive(false);
        if (itemIdx == 1)
        {
            item[3].SetActive(true);
        }
        
        E_turn = true;
        myturn = false;
    }
    IEnumerator Block()
    {
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("Deffend");
        Character[0].gameObject.GetComponent<Animator>().SetTrigger("Deffend2Idle");
        yield return new WaitForSeconds(2.15f);

        inventory.SetActive(true);
        E_turn = true;
        myturn = false;
    }
    IEnumerator MessageAppend()
    {
        fightInfoM.text = getFightInfo;
        yield return new WaitForSeconds(5.0f);
        fightInfoM.text = "";
    }
}