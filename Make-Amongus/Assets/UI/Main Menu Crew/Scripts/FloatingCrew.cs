using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrew : MonoBehaviour
{
    public EPlayerColor playerColor;
    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private float floatingSpeed;
    private float rotateSpeed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();        
    }

    public void SetFloatingCrew(Sprite sprite ,
        EPlayerColor playerColor,
        Vector3 direction, float floatingSpeed,
        float rotateSpeed, float size)
    {
        this.playerColor = playerColor;
        this.direction = direction;
        this.floatingSpeed = floatingSpeed;
        this.rotateSpeed = rotateSpeed;

        spriteRenderer.sprite = sprite;
        spriteRenderer.material.SetColor("_PlayerColor", PlayerColor.GetColor(playerColor));

        transform.localScale = new Vector3(size, size, size); // size 를이용해 크루원 크기정하기

        spriteRenderer.sortingOrder = (int)Mathf.Lerp(1, 32767, size); //크기가큰 크루원이 앞으로나오고 크기가작은 크루원을 가리게만들기
    }

    void Update()
    {
        transform.position += direction * floatingSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(transform.transform.eulerAngles + new Vector3(0f, 0f, rotateSpeed));
    }
}
