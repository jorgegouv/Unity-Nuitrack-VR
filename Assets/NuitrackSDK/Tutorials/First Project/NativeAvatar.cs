#region Description

// The script performs a direct translation of the skeleton using only the position data of the joints.
// Objects in the skeleton will be created when the scene starts.

#endregion


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[AddComponentMenu("Nuitrack/Example/TranslationAvatar")]
public class NativeAvatar : MonoBehaviour
{
    string message = "";

    public nuitrack.JointType[] typeJoint;
    GameObject[] CreatedJoint;
    public GameObject PrefabJoint;

    /*const float scalK = 0.01f;
    public bool validaPosicoes = true;
    private float timerLePosicao = 5f;
    public Text TimerPosicaoInicial;
    public Text Agachamentos, Limite;

    Vector3[] PosicoesIniciais = new Vector3[4];
    public bool agachou = false;
    public int agachamentoCounter = 0;*/

    const float scalK = 0.01f;
    private bool isPosicaoInicialGuardada = false, isAgachado = false;
    Vector3[] coordenadasJoints = new Vector3[4];
    public Text timerMensagem, contadorAgachamentos;
    private float timerComecar = 5f, timerDescanso = 15f;
    private float[] posInicial_Y;
    private int agachamentos = 10, serie = 0;

    void Start()
    {
        CreatedJoint = new GameObject[typeJoint.Length];
        for (int q = 0; q < typeJoint.Length; q++)
        {
            CreatedJoint[q] = Instantiate(PrefabJoint);
            CreatedJoint[q].transform.SetParent(transform);
        }
        message = "Skeleton created";

        timerMensagem.text = "Coloque-se na posição inicial!";

    }

    void Update()
    {
        if (CurrentUserTracker.CurrentUser != 0)
        {
            nuitrack.Skeleton skeleton = CurrentUserTracker.CurrentSkeleton;
            message = "Skeleton found";
            
            for (int q = 0; q < typeJoint.Length; q++)
            {
                nuitrack.Joint joint = skeleton.GetJoint(typeJoint[q]);
                Vector3 newPosition = 0.001f * joint.ToVector3();
                CreatedJoint[q].transform.localPosition = newPosition;
            }

            contadorAgachamentos.text = agachamentos.ToString(); //atualiza mensagem dos agachamentos

            coordenadasJoints[0] = scalK * skeleton.GetJoint(nuitrack.JointType.Head).ToVector3();
            coordenadasJoints[1] = scalK * skeleton.GetJoint(nuitrack.JointType.Neck).ToVector3();
            coordenadasJoints[2] = scalK * skeleton.GetJoint(nuitrack.JointType.RightShoulder).ToVector3();
            coordenadasJoints[3] = scalK * skeleton.GetJoint(nuitrack.JointType.LeftShoulder).ToVector3();

            if(!isPosicaoInicialGuardada){ //inicia a contagem decrescente para guardar a posição inicial e iniciar o exercício
                timerComecar -= Time.deltaTime;;
                timerMensagem.text = timerComecar.ToString();
                
                if(timerComecar <0)
                    GuardaPosicaoInicial(coordenadasJoints);

                return;
            }
            
            if(agachamentos > 0)
            {
                VerificaAgachamento(coordenadasJoints);

                if(isAgachado){
                    ContaAgachamento(coordenadasJoints);
                }
            }
            else{
                serie++;
                DescansaProximaSerie();
                
            }


            /*Vector3 OmbroDireitoPosition = scalK * skeleton.GetJoint(nuitrack.JointType.RightShoulder).ToVector3();
            Vector3 OmbroEsquerdoPosition = scalK * skeleton.GetJoint(nuitrack.JointType.LeftShoulder).ToVector3();

            Vector3 PescocoPosition = scalK * skeleton.GetJoint(nuitrack.JointType.Neck).ToVector3();
            Vector3 CabecaPosition = scalK * skeleton.GetJoint(nuitrack.JointType.Head).ToVector3();

            if (validaPosicoes)
            {
                timerLePosicao -= Time.deltaTime;
                TimerPosicaoInicial.text = timerLePosicao.ToString();

                if(timerLePosicao < 0)
                {
                    
                    TimerPosicaoInicial.text = "Ta Lá";
                    validaPosicoes = false;
                    PosicoesIniciais[0] = OmbroDireitoPosition;
                    PosicoesIniciais[1] = OmbroEsquerdoPosition;
                    PosicoesIniciais[2] = PescocoPosition;
                    PosicoesIniciais[3] = CabecaPosition;
                }
            }
            else
            {
                float distAgachamentoOmbroDireito = PosicoesIniciais[0].y - OmbroDireitoPosition.y;
                float distAgachamentoOmbroEsquerdo = PosicoesIniciais[1].y - OmbroEsquerdoPosition.y;

                float distAgachamentoPescoco = PosicoesIniciais[2].y - PescocoPosition.y;
                float distAgachamentoCabeca = PosicoesIniciais[3].y - CabecaPosition.y;

                if (!agachou)
                {
                    if ((distAgachamentoOmbroDireito > 3.0f) && (distAgachamentoOmbroEsquerdo > 3.0f) && (distAgachamentoPescoco > 2.0f) && (distAgachamentoCabeca > 2.0f))
                    {
                        agachou = true;
                        Limite.text = agachou.ToString();
                    }
                        
                }
                else
                {
                    if ((distAgachamentoOmbroDireito < 0.5f) && (distAgachamentoOmbroEsquerdo < 0.5f) && (distAgachamentoPescoco < 0.5f) && (distAgachamentoCabeca < 0.5f))
                    {
                        agachou = false;
                        Limite.text = agachou.ToString();
                        agachamentoCounter++;
                        print("TAI: " + agachamentoCounter);
                        Agachamentos.text = agachamentoCounter.ToString();
                    }
                }
            }

            print("Direito: " + OmbroDireitoPosition);
            print("Esquerdo: " + OmbroEsquerdoPosition);
            print("Pescoco: " + PescocoPosition);
            print("Cabeca: " + CabecaPosition);*/

        }
        else
        {
            message = "Skeleton not found!";
        }
    }

    private void DescansaProximaSerie()
    {
        if(serie == 3)
            contadorAgachamentos.text = "Acabou o exercício!";
        else {
            contadorAgachamentos.text = "Terminas-te a " + serie + "ª série!";

            timerDescanso -= Time.deltaTime;;
            timerMensagem.text = timerComecar.ToString();
            if(timerDescanso < 0)
            {
                timerMensagem.text = "Vamos lá!";
                agachamentos = 10;
            }
        }
    }

    private void ContaAgachamento(Vector3[] coordenadasJoints)
    {
        //calcula se voltou as unidades iniciais
        for(int i=0; i<4;i++){
            float posDelta = posInicial_Y[i] - coordenadasJoints[i].y;
            
            if(posDelta > 0.5f)
                return;
        }
        
        isAgachado = false;
        agachamentos--;
    }

    private void VerificaAgachamento(Vector3[] coordenadasJoints)
    {
        //calcula quantas unidades desceu
        for(int i=0; i<4;i++){
            float posDelta = posInicial_Y[i] - coordenadasJoints[i].y;
            
            if(posDelta < 3.0f)
                return;
        }
        
        isAgachado = true;
    }

    private void GuardaPosicaoInicial(Vector3[] coordenadasJoints)
    {
        for(int i=0; i<4;i++){ // cabeça; pescoço; ombro direito; ombro esquerdo
            posInicial_Y[i] = coordenadasJoints[i].y;
        }

        timerMensagem.text = "Vamos lá!";
        isPosicaoInicialGuardada = true;
    }



    // Display the message on the screen
    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 50;
        GUILayout.Label(message);
    }
}