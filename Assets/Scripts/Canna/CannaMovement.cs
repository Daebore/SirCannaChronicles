using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CannaMovement : MonoBehaviour
{
    [SerializeField] public Transform controladorGolpe;
    [SerializeField] public Transform controladorGolpe2;


    [SerializeField] public float radioGolpe;
    [SerializeField] public float danyoGolpe;

    [SerializeField] public Vector2 dimensionesCaja;
    [SerializeField] public Transform posicionCaja;




    public bool isAttacking = false;


    private Rigidbody2D rb2D;

    [SerializeField] private float vida = 200;

    //no sirve para nada, es solo para ordenar el codigo, como si fuera el Head del html
    [Header("Movimiento")]
    
    private float MovimientoHorizontal = 0f;

    //SerializeField sirve para que muestre en el IDE de Unity la variable, como si estuviera en publica la variable
    [SerializeField] private float velocidadDeMovimiento = 110;




    //suavizadoDeSuelo sirve para que el movimiento no sea tan brusco
    //Range sirve para poner un range entre dos numeros
    [Range(0, 1f)][SerializeField] private float suavizadoDeMovimiento =0.1f;

    private Vector3 velocidad = Vector3.zero;

    private bool mirandoDerecha = true;


    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto = 160 ;

    //controla que es saltable y que no
    [SerializeField] private LayerMask queEsSuelo;

    [SerializeField] private Transform controladorSuelo;

    [SerializeField] private Vector3 dimensionCaja;

    [SerializeField] private bool enSuelo;

    private bool salto = false;

    public static CannaMovement instance;

    [Header("Animacion")]
    public Animator animator;


    private void Awake()
    {
        instance = this; 
    }



    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        Attack();

        MovimientoHorizontal = Input.GetAxisRaw("Horizontal") * velocidadDeMovimiento;

        animator.SetFloat("Horizontal", Mathf.Abs(MovimientoHorizontal));

        animator.SetFloat("VelocidadY", rb2D.velocity.y);

        if (Input.GetButtonDown("Jump")) { 
            salto = true;
        }


    }


    void Attack() {

        if (Input.GetKeyDown(KeyCode.F) && !isAttacking) {
            
            isAttacking = true;

        }




    }


    //como el update(), pero orientado a cambios de fisicas
    private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionCaja, 0f, queEsSuelo);

        animator.SetBool("enSuelo", enSuelo);



        //mover para cualquier equipo
        Mover(MovimientoHorizontal * Time.fixedDeltaTime, salto);




        salto = false;
    }

    private void Mover(float mover, bool saltar) {

        Vector3 velocidadObjetivo = new Vector2(mover, rb2D.velocity.y);

        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);

        if (mover > 0 && !mirandoDerecha) {
            Girar();
        }
        else if (mover <0 && mirandoDerecha) {
            Girar();
        }



        if (enSuelo && saltar) {
            enSuelo = false;
            rb2D.AddForce(new Vector2(0f, fuerzaDeSalto));
        }



    }


    public void RecibirDanyo(float danyo)
    {
        animator.SetTrigger("Hit");
        vida -= danyo;

        if(vida <= 0)
        {
            Muerte();
        }


    }

    public void RecuperaVida(int salud)
    {
        this.vida = this.vida + salud;
    }


    public void Muerte()
    {
        animator.SetTrigger("Muerte");
        Destroy(gameObject, 5f);
    }



    private void Girar() {

        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;

        escala.x *= -1;
        transform.localScale = escala;
    
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionCaja);


        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);

        Gizmos.DrawWireCube(controladorGolpe2.position, dimensionesCaja);

    }


    


}