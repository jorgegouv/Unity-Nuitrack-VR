#region Description

// The script performs a direct translation of the skeleton using only the position data of the joints.
// Objects in the skeleton will be created when the scene starts.

#endregion


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("Nuitrack/Example/TranslationAvatar")]
public class NativeAvatar : MonoBehaviour
{
    string message = "";

    public nuitrack.JointType[] typeJoint;
    GameObject[] CreatedJoint;
    public GameObject PrefabJoint;

    const float scalK = 0.01f;
    public bool validaPosicoes = true;
    private float timerLePosicao = 5f;
    public Text TimerPosicaoInicial;
    public Text Agachamentos, Limite;

    Vector3[] PosicoesIniciais = new Vector3[4];
    public bool agachou = false;
    public int agachamentoCounter = 0;

    void Start()
    {
        CreatedJoint = new GameObject[typeJoint.Length];
        for (int q = 0; q < typeJoint.Length; q++)
        {
            CreatedJoint[q] = Instantiate(PrefabJoint);
            CreatedJoint[q].transform.SetParent(transform);
        }
        message = "Skeleton created";

        TimerPosicaoInicial.text = "Posição Inicial";

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

            Vector3 OmbroDireitoPosition = scalK * skeleton.GetJoint(nuitrack.JointType.RightShoulder).ToVector3();
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
                float distAgachamentoOmbroDireito = PosicoesIniciais[0].y - OmbroDireitoPosition.y;/*Vector3.Distance(PosicoesIniciais[0], OmbroDireitoPosition);*/
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
            print("Cabeca: " + CabecaPosition);

        }
        else
        {
            message = "Skeleton not found";
        }
    }

    // Display the message on the screen
    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 50;
        GUILayout.Label(message);
    }
}