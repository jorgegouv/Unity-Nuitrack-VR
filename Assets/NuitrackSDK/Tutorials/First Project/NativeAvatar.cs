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

    private const int nAgachamentos= 10; //agachamentos a fazer
    private const float tempoDescanso = 15f; //tempo de descanso entre as series
    private const int totalSeries = 3; //numero total de series a fazer
    private int serie = 1;

    const float scalK = 0.01f;
    private bool isPosicaoInicialGuardada = false, isAgachado = false;
    Vector3[] coordenadasJoints = new Vector3[4];
    public Text timerMensagem, contadorAgachamentos;
    private float timerComecar = 5f;
    
    public float[] posInicial_Y = new float[3];
    


    private int agachamentos = nAgachamentos;
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
                DescansaProximaSerie();
                
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
            contadorAgachamentos.text = "Acabou o exercício!";
        else {
            contadorAgachamentos.text = "Terminas-te a " + serie + " série!";
            
            timerDescanso -= Time.deltaTime;
            timerMensagem.text = "Descanso:" + timerDescanso.ToString();

            if(timerDescanso < 0){
                timerMensagem.text = "Vamos lá!";
                agachamentos = nAgachamentos;
                serie++;
                timerDescanso = tempoDescanso;
            }
            
        }
    }

    private void ContaAgachamento(Vector3[] coordenadasJoints)
    {
        //calcula se voltou as unidades iniciais
        for(int i=0; i<3;i++){
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
        for(int i=0; i<3;i++){
            float posDelta = posInicial_Y[i] - coordenadasJoints[i].y;
            
            if(posDelta < 3.0f)
                return;
        }
        
        isAgachado = true;
    }

    private void GuardaPosicaoInicial(Vector3[] coordenadasJoints)
    {
        for(int i=0; i<3;i++){ // cabeça; pescoço; ombro direito; ombro esquerdo
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