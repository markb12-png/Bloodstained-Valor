using UnityEngine;

public class Player : MonoBehaviour {

    public enum Move {idle, isJumping, isRunning, isWalking}

    // Player states
    public static bool isGrounded;
    public static bool isAttacking;
    public static Move state = Move.idle;

    void Start() {
        Init();
    }

    private static void Init() {}
}
