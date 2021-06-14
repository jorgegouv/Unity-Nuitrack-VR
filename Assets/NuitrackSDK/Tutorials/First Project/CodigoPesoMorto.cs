#region Description

// The script performs a direct translation of the skeleton using only the position data of the joints.
// Objects in the skeleton will be created when the scene starts.

#endregion


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[AddComponentMenu("Nuitrack/Example/TranslationAvatar")]
public class CodigoPesoMorto : MonoBehaviour
{
    string message = "";

    public nuitrack.JointType[] typeJoint;
    GameObject[] CreatedJoint;
    public GameObject PrefabJoint;

    private const int nRepeticoes= 10; //repetiçoes a fazer cada perna
    private const float tempoDescanso = 15f; //tempo de descanso entre as series
    private const int totalSeries = 3; //numero total de series a fazer
    private int serie = 1;

    private bool trocouPerna = false;

    const float scalK = 0.01f;
    private bool isPosicaoInicialGuardada = false, isAgachado = false;
    Vector3[] coordenadasJoints = new Vector3[8];
    public Text timerMensagem, contadorRepeticoes;
    private float timerComecar = 5f;
    
    Vector3[] posInicial_Y = new Vector3[8];
    


    private int repeticoes = nRepeticoes;
    private float timerDescanso = tempoDescanso;
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
            message = "";
            
            for (int q = 0; q < typeJoint.Length; q++)
            {
                nuitrack.Joint joint = skeleton.GetJoint(typeJoint[q]);
                Vector3 newPosition = 0.001f * joint.ToVector3();
                CreatedJoint[q].transform.localPosition = newPosition;
            }

            contadorRepeticoes.text = repeticoes.ToString(); //atualiza mensagem dos agachamentos

            coordenadasJoints[0] = scalK * skeleton.GetJoint(nuitrack.JointType.Head).ToVector3(); //Cabeça
            coordenadasJoints[1] = scalK * skeleton.GetJoint(nuitrack.JointType.Neck).ToVector3(); //Pescoço
            coordenadasJoints[2] = scalK * skeleton.GetJoint(nuitrack.JointType.RightShoulder).ToVector3(); //ombro direito
            coordenadasJoints[3] = scalK * skeleton.GetJoint(nuitrack.JointType.LeftShoulder).ToVector3(); //ombro esquerdo
            coordenadasJoints[4] = scalK * skeleton.GetJoint(nuitrack.JointType.Torso).ToVector3(); //torso
            coordenadasJoints[5] = scalK * skeleton.GetJoint(nuitrack.JointType.LeftElbow).ToVector3(); //braco direito
            coordenadasJoints[6] = scalK * skeleton.GetJoint(nuitrack.JointType.LeftElbow).ToVector3(); //braco esquerdo
           
            coordenadasJoints[7] = scalK * skeleton.GetJoint(nuitrack.JointType.RightKnee).ToVector3(); //joelho direito
            coordenadasJoints[8] = scalK * skeleton.GetJoint(nuitrack.JointType.LeftKnee).ToVector3(); //joelho esquerdo

            
            print("CABECA: " + coordenadasJoints[0]);
            print("PESCOCO: " + coordenadasJoints[1]);
            print("OMBRO DIREITO: " + coordenadasJoints[2]);
            print("OMBRO ESQUERDO: " + coordenadasJoints[3]);
            print("TORSO: " + coordenadasJoints[4]);
            print("BRACO DIREITO: " + coordenadasJoints[5]);
            
            print("BRACO ESQUERDO: " + coordenadasJoints[6]);
            print("JOELHO DIREITO: " + coordenadasJoints[7]);
            print("JOELHO ESQUERDO: " + coordenadasJoints[8]);


            if(!isPosicaoInicialGuardada){ //inicia a contagem decrescente para guardar a posição inicial e iniciar o exercício
                timerComecar -= Time.deltaTime;;
                timerMensagem.text = timerComecar.ToString();
                
                if(timerComecar <0)
                    GuardaPosicaoInicial(coordenadasJoints);

                return;
            }
            
            
            if(repeticoes > 0)
            {
                VerificaRepeticao(coordenadasJoints);

                if(isAgachado){
                    ContaAgachamento(coordenadasJoints);
                }
            }
            else{

                if(trocouPerna)
                    DescansaProximaSerie();
                else{ //troca de perna
                    repeticoes = nRepeticoes;
                    trocouPerna = true;
                    timerMensagem.text = "Vamos lá! Lentanta a perna direita";
                }
                
            }

        }
        else
        {
            message = "Skeleton not found!";
        }
    }

    private void DescansaProximaSerie()
    {
        if(serie == totalSeries)
            contadorRepeticoes.text = "Acabou o exercício!";
        else {
            contadorRepeticoes.text = "Terminas-te a " + serie + " série\n Volta à levantar a perna esquerda!";
            
            timerDescanso -= Time.deltaTime;
            timerMensagem.text = "Descanso: " + timerDescanso.ToString();

            if(timerDescanso < 0){
                timerMensagem.text = "Vamos lá! Lentanta a perna esquerda";
                repeticoes = nRepeticoes;
                trocouPerna = false;
                serie++;
                timerDescanso = tempoDescanso;
            }
            
        }
    }

    private void ContaAgachamento(Vector3[] coordenadasJoints)
    {
        //calcula se voltou as unidades iniciais
        for(int i=0; i<6;i++){
            float posDeltaY = posInicial_Y[i].y - coordenadasJoints[i].y;
            float posDeltaZ = posInicial_Y[i].z - coordenadasJoints[i].z;
            
            if(posDeltaY > 0.5f && posDeltaZ > 0.5f)
                return;
        }

        if(trocouPerna)
        {
            if(posInicial_Y[8].y - coordenadasJoints[8].y > 1.0f && posInicial_Y[8].z - coordenadasJoints[8].z > 1.0f) return; //mexeu a perna esquerda nao conta
        }
        else{
            if(posInicial_Y[7].y - coordenadasJoints[7].y > 1.0f && posInicial_Y[7].z - coordenadasJoints[7].z > 1.0f) return; //mexeu a perna direita nao conta
        }
        
        isAgachado = false;
        repeticoes--;
    }

    private void VerificaRepeticao(Vector3[] coordenadasJoints)
    {
        //verifica se desceu as unidades necessárias no y e no z
        for(int i=0; i<6;i++){
            float posDeltaY = posInicial_Y[i].y - coordenadasJoints[i].y;
            float posDeltaZ = posInicial_Y[i].z - coordenadasJoints[i].z;
            
            if(posDeltaY < 5.0f && posDeltaZ < 5.0f)
                return;
        }

        if(trocouPerna)
        {
            if(posInicial_Y[8].y - coordenadasJoints[8].y > 1.0f && posInicial_Y[8].z - coordenadasJoints[8].z > 1.0f) return; //mexeu a perna esquerda nao conta
        }
        else{
            if(posInicial_Y[7].y - coordenadasJoints[7].y > 1.0f && posInicial_Y[7].z - coordenadasJoints[7].z > 1.0f) return; //mexeu a perna direita nao conta
        }

        
        isAgachado = true;
    }

    private void GuardaPosicaoInicial(Vector3[] coordenadasJoints)
    {
        for(int i=0; i<7;i++){ // cabeça; pescoço; ombro direito; ombro esquerdo
            posInicial_Y[i] = coordenadasJoints[i];
        }

        timerMensagem.text = "Vamos lá! Levanta a perna Esquerda";
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