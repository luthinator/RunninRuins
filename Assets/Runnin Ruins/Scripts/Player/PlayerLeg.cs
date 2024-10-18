using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using static UnityEngine.UI.Image;

public class PlayerLeg : MonoBehaviour
{
    [SerializeField] Transform footTarget;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] PlayerLeg otherFoot = default;
    [SerializeField] Player player;
    [SerializeField] Transform FootTransform;
    public float speed = 1;
    public float stepDistance = 1;
    public float stepHeight = 0.5f;
    public bool dominantLeg;
    public float wallOffset = 0.4f;
    
    public Vector3 legPosition, newPosition, highlightedNewPosition, oldPosition, savedPosition;
    public float lerp;
    public float lerp2;

    // Start is called before the first frame update
    void Start()
    {
        ResetLeg();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player.playerFlipped)
        {
            ResetLeg();
        }

        if (lerp2 < 1)
        {
            // TODO: prevent janky movement by testing the previous legPosition and measuring the distance
            transform.position = legPosition;
        }

        if (player.GetMoveInput() == 0)
            StandingStill();
        else
            WallCheck();

        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(highlightedNewPosition, 0.1f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }

    public void WallCheck()
    {
        Vector3 wallCheck = new Vector3(player.transform.position.x, footTarget.position.y, footTarget.position.z);
        Ray ray = new Ray(wallCheck, Vector3.right * -player.direction);
        if (Physics.Raycast(ray, out RaycastHit info, Vector3.Distance(wallCheck, footTarget.position), groundLayer))
        {
            Vector3 newTarget = info.point;
            newTarget.x += wallOffset * player.direction;
            PlayerMove(newTarget);
        }
        else
        {
            Vector3 newTarget = footTarget.position;
            newTarget.x += wallOffset * player.direction;
            PlayerMove(newTarget);
        }
    }
    public void PlayerMove(Vector3 target)
    {
        Ray ray = new Ray(target, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 3, groundLayer))
        {
            highlightedNewPosition = info.point;
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                newPosition = info.point;
            }
        }

        // TODO: Add animation when the player is in the air

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            legPosition = tempPosition;
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
        }
        
        savedPosition = transform.position;
        lerp2 = 0;
    }

    public void StandingStill()
    {
        lerp = 1;

        if (lerp2 < 1)
        {
            Vector3 playerPosition = player.GetComponent<Transform>().position;
            playerPosition = new Vector3(playerPosition.x - 0.2f, playerPosition.y, transform.position.z);
            savedPosition = new Vector3(savedPosition.x, transform.position.y, savedPosition.z);
            Vector3 tempPosition = Vector3.Lerp(savedPosition, playerPosition, lerp2);

            legPosition = tempPosition;
            lerp2 += Time.deltaTime * speed;
        }
    }

    public void ResetLeg()
    {
        lerp = 1;
        lerp2 = 1;
        Vector3 resetVector = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.position = resetVector;
        newPosition = resetVector;
        oldPosition = resetVector;
        legPosition = resetVector;
        savedPosition = resetVector;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(FootTransform.position, Vector3.down, 0.6f, groundLayer);
    }
}
