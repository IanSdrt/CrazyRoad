using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Duck : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float moveDuration = 0.1f;
    [SerializeField, Range(0, 1)] float jumpHeight = 0.5f;

    [SerializeField] int leftMoveLimit;
    [SerializeField] int rightMoveLimit;
    [SerializeField] int backMoveLimit;
    [SerializeField] UnityEvent JumpSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] AudioClip DieSound;

    public UnityEvent<Vector3> OnJumpEnd;
    public UnityEvent<int> OnGetCoin;
    public UnityEvent OnDie;

    private bool isMoveable = false;

    void Update()
    {
        if (isMoveable == false)
            return;

        if (DOTween.IsTweening(transform))
            return;

        Vector3 direction = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction += Vector3.forward;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction += Vector3.back;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction += Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction += Vector3.left;
        }

        if (direction == Vector3.zero)
            return;

        Move(direction);
    }

    public void Move(Vector3 direction)
    {
        var targetPosition = transform.position + direction;
        if( targetPosition.x < leftMoveLimit || 
            targetPosition.x > rightMoveLimit || 
            targetPosition.z < backMoveLimit ||
            Tree.AllPositions.Contains(targetPosition))
        {
            targetPosition = transform.position;
        }

        transform.DOJump(
        targetPosition, 
        jumpHeight, 
        1, 
        moveDuration)
        .onComplete = BroadCastPositionOnJumpEnd;

        transform.forward = direction;
        JumpSound.Invoke();
    }

    public void SetMoveable(bool value)
    {
       isMoveable = value; 
    }

    public void UpdateMoveLimit(int horizontalSize, int backLimit)
    {
        leftMoveLimit = -horizontalSize / 2;
        rightMoveLimit = horizontalSize / 2;
        backMoveLimit = backLimit;
    }

    private void BroadCastPositionOnJumpEnd()
    {
        OnJumpEnd.Invoke(transform.position);
    }

    private bool isDead = false;

private void OnTriggerEnter(Collider other) 
{
    if(other.CompareTag("Car"))
    {
        if(transform.localScale.y == 0.1f)
            return;

        transform.DOScale(new Vector3(2,0.1f,1),0.2f);

        isMoveable = false;
        Die(); // Panggil langsung fungsi Die() tanpa menggunakan Invoke()
    }
    else if (other.CompareTag("Eagle"))
    {
        if(this.transform != other.transform)
        {
            this.transform.SetParent(other.transform);
            Die(); // Panggil langsung fungsi Die() tanpa menggunakan Invoke()
        }
    }
} 

    private void Die()
    {
        if (isDead) return; // Cek apakah karakter sudah mati sebelumnya

        isDead = true; // Menandakan karakter telah mati

        OnDie.Invoke();

        if (DieSound != null)
        {
            audioSource.PlayOneShot(DieSound);
        }
    }
}
