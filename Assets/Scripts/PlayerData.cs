using UnityEngine;

public class Player : MonoBehaviour {

    public enum Move {idle, isJumping, isRunning, isWalking}

    // Player states
    public static bool isGrounded;
    public static Move state = Move.idle;

    public static bool isAttacking;
    public static bool canAttack;

    void Start() {
        Init();
    }

    private static void Init() {}
}
