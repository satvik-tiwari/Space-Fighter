using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] float backgroundScrollSpeed = 0.5f;
    [SerializeField] float incrementBackgroundScrollSpeed = 0.1f;
   
    [SerializeField] float limitBackgroundScrollSpeed = 5f;

    
    Material myMaterial;
    [SerializeField]Vector2 offset;
    [SerializeField]bool isStartWarp = false;
    [SerializeField]bool isEndWarp = false;

    // Start is called before the first frame update
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        if(!isStartWarp && !isEndWarp)
            offset = new Vector2(0f, backgroundScrollSpeed);
        
    }

    // Update is called once per frame
    void Update()
    {
       
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
        if(isStartWarp)
        {
            StartWarp();
        }
        else if (isEndWarp)
        {
            EndWarp();
        }

    }

    private void EndWarp()
    {
        offset -= new Vector2(0f, incrementBackgroundScrollSpeed) * Time.deltaTime;
        offset.y = Mathf.Clamp(offset.y, backgroundScrollSpeed, limitBackgroundScrollSpeed);
    }

    private void StartWarp()
    {
        
        offset += new Vector2(0f, incrementBackgroundScrollSpeed) * Time.deltaTime;
        offset.y = Mathf.Clamp(offset.y, backgroundScrollSpeed, limitBackgroundScrollSpeed);
        
        
    }

    public void SetStartWarp(bool isStartWarp)
    {
        this.isStartWarp = isStartWarp;
    }

    public void SetEndWarp(bool isEndWarp)
    {
        if(isEndWarp)
            offset = new Vector2(0f, limitBackgroundScrollSpeed);
        else
            offset = new Vector2(0f, backgroundScrollSpeed);
        this.isEndWarp = isEndWarp;
    }
}
