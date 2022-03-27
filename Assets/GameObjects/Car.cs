
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public GameObject map;

    //     # 小车对象，每一个实例代表一个小车
    // # 每辆小车具有一个路径列表，当运行时，会占用列表中的2项路径点，停止时，会占用其中的1项路径点
    // # 由于多机情况下的死锁可能问题，每当一个小车到达一个路径点，会需要更新下一个路径点
    // # 就需要对全局路径规划进行“银行家算法”的安全判定
    // # 如果失败，那么小车只能停在当前点，不可以前往下一个点

    public int holdTagNow;
    public int holdTagNext;
    public List<int> pathList;
    public int listIndex;
    public int state = 0;

    public float moveSpeed = 0.5f;//1s

    private void Start() {

    }
    public void InitParm(int initTag){
        holdTagNow = initTag;
        holdTagNext = -1;
    }

    private void Update() {
        switch(state){
            case 0:{
                transform.position = map.GetComponent<Map>().pointList[holdTagNow].transform.position;
                break;}
            case 1:{
                transform.position = transform.position + 
                    Time.deltaTime * moveSpeed * (map.GetComponent<Map>().pointList[holdTagNext].transform.position - transform.position).normalized;
                if(Vector3.Distance(transform.position, map.GetComponent<Map>().pointList[holdTagNext].transform.position) < 0.005f){
                    TagArrived();
                }
                break;}
            case 2:{
                //map.GetComponent<Map>().TellWaitingCarToGoIfAllWaiting();
                //just hold on
                break;}
        }
    }


    private void TagArrived(){
        // Debug.Log("Tag Arrived " + holdTagNext);
        holdTagNow = holdTagNext;
        if(pathList.Count>=1){
            holdTagNext = pathList[0];
            pathList.RemoveAt(0);
            TellCheckToGo();
        }else{
            holdTagNext = -1;
            state = 0;
        }
    }

    public void TellCheckToGo(){
        state = 1;
        if(!map.GetComponent<Map>().BankerSaver(listIndex)){
            state = 2;
            map.GetComponent<Map>().TellWaitingCarToGoIfAllWaiting();
        }else{
            state = 1;
            map.GetComponent<Map>().TellWaitingCarToGo();
        }
    }


    public void SetPathList(List<int> _pathList){
        this.pathList = _pathList;
        if(state == 0){
            holdTagNext = this.pathList[1];
            this.pathList.RemoveAt(0);
            this.pathList.RemoveAt(0);
            state = 2;//一开始都暂停住
        }
        map.GetComponent<Map>().TellWaitingCarToGoIfAllWaiting();
    }

}
