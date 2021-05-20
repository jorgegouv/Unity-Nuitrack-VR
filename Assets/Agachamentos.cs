using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Agachamentos : MonoBehaviour
{
     public enum Direction
    {
        Stand = 0,
        Left = 1,
        Right,
        Jump,
        Count = 4
    }

    [Header("Main")]
    [SerializeField] nuitrack.JointType targetJoint = nuitrack.JointType.LeftKnee;
    [SerializeField] float deltaVelocity = 0.25f;

    [SerializeField] nuitrack.JointType targetJointRightKnee = nuitrack.JointType.RightKnee;


    const float scalK = 0.001f;
    public GameObject Floor;
    
    float tamanhoPernas;
    [Header("Test output")]
    [SerializeField] Text text;
    [SerializeField] Text text1;
    [SerializeField] Text text2;
    [SerializeField] Text Score;
    [SerializeField] Text EstadoAtual;

    public int score = 0;

    public bool validarapenas1vezPerna = true;

    public bool podeInstanciar1 = true, podeInstanciar2 = true;

    public int apanharBola = 0;
    public int naoapanharBola = 0;

    public int LungeBemFeito, ContagemDeLunges;
    public bool lungebool = true, lungebool2 = true;

    public int lancamentosBowling = 0;

    public bool ladoDireito = true, ladoEsquerdo = true;


    public Transform LeftHand, RightHand;

    public bool maodireitalancamento, maoesquerdalancamento;
    public bool criarboladireita, criarbolaesquerda;

    private Vector3 posicaoInicialLancamento;
    public bool posicaoinicialLadoDireito, posicaoinicialLadoEsquerdo;

    

    public float forcadeLancamento = 20f;

    public bool tem2lancamentos; //para mostrar que como tem 2 lancamentos para colocar bola para lancar no lado esquerdo
    public float timer = 4f;
    public bool TempoParaRestaurarPinos;
    private List<Vector3> pinPositions;
    private List<Quaternion> pinRotations;


    private float timerTamanhoPernas = 5f;


    // Public property
    public Direction MoveDirection
    {
        get;
        private set;
    }

    void Start()
    {
        /*var pins = GameObject.FindGameObjectsWithTag("Pin");
        pinPositions = new List<Vector3>();
        pinRotations = new List<Quaternion>();
        foreach (var pin in pins)
        {
            pinPositions.Add(pin.transform.position);
            pinRotations.Add(pin.transform.rotation);
        }*/

        NuitrackManager.onSkeletonTrackerUpdate += NuitrackManager_onSkeletonTrackerUpdate;
    }

    void NuitrackManager_onSkeletonTrackerUpdate(nuitrack.SkeletonData skeletonData)
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            var pins = GameObject.FindGameObjectsWithTag("Pin");

            for(int i = 0; i < pins.Length; i++)
            {
                var pinPhysics = pins[i].GetComponent<Rigidbody>();
                pinPhysics.velocity = Vector3.zero;
                pinPhysics.position = pinPositions[i];
                pinPhysics.rotation = pinRotations[i];
                pinPhysics.velocity = Vector3.zero;
                pinPhysics.angularVelocity = Vector3.zero;
            }

        }
            
        if(CurrentUserTracker.CurrentUser != 0)
        {
            nuitrack.Skeleton skeleton = CurrentUserTracker.CurrentSkeleton;

            Vector3 leftKneePosition = scalK * skeleton.GetJoint(nuitrack.JointType.LeftKnee).ToVector3();

            Vector3 rightKneePosition = scalK * skeleton.GetJoint(nuitrack.JointType.RightKnee).ToVector3();

            float distSizeofLeg = Vector3.Distance(rightKneePosition, Floor.gameObject.transform.position);

            float distJoelhoEsqaoChao = Vector3.Distance(Floor.gameObject.transform.position, leftKneePosition);

            float distJoelhoDiraoChao = Vector3.Distance(Floor.gameObject.transform.position, rightKneePosition);





            //Timer para guardar tamanho das pernas
            if (validarapenas1vezPerna)
            {
                timerTamanhoPernas -= Time.deltaTime;
                EstadoAtual.text = "Timer: " + timerTamanhoPernas;
                if (timerTamanhoPernas <- 0)
                {
                    tamanhoPernas = distSizeofLeg;
                    validarapenas1vezPerna = false;
                    EstadoAtual.text = "Tamanho da perna: " + tamanhoPernas;
                }
                
            }
            
            if(tamanhoPernas > 0)
            {
                //Lado Direito
                Vector3 targetPostion = scalK * skeleton.GetJoint(targetJoint).ToVector3();
                Vector3 moveDirection = targetPostion - rightKneePosition;

                Vector3 velocityMove = Vector3.Project(rightKneePosition.normalized, moveDirection);


                float distSeparacaodePernas = Vector3.Distance(rightKneePosition, targetPostion);

                //Lado Esq
                Vector3 targetPostionEsq = scalK * skeleton.GetJoint(targetJointRightKnee).ToVector3();
                Vector3 moveDirectionEsq = targetPostionEsq - leftKneePosition;

                Vector3 velocityMoveEsq = Vector3.Project(leftKneePosition, moveDirectionEsq);

                float distSeparacaodePernasEsq = Vector3.Distance(leftKneePosition, targetPostionEsq);




                //Debug.Log("Tamanho da perna: " + tamanhoPernas);
                //Debug.Log("Separacao das pernas: " + distSeparacaodePernasEsq);
                //Debug.Log("Distancia Joelho Direito ao chao:" + distJoelhoDiraoChao);
                //Debug.Log("Distancia Joelho Esquerdo ao chao:" + distJoelhoEsqaoChao);


                
                //text.text = "" + distJoelhoDiraoChao;
			}
		}
    }

    public void ScoreIncrease()
    {
        score += 20;
        Score.text = "Score: " + score;
    }

    public void CatchBall()
    {
        apanharBola++;
    }

    public void DidntCatchBall()
    {
        naoapanharBola++;
    }

}