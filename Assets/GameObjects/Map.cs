using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public List<GameObject> pointList;

    public List<List<int>> p2pMap; 

    public List<GameObject> carList;
    public List<Car> carCtrlList = new List<Car>();

    private void Start() {
        for (int i=0;i<carList.Count;i++){
            carList[i].GetComponent<Car>().listIndex = i;
            carCtrlList.Add(carList[i].GetComponent<Car>());
        }
        p2pMap =new List<List<int>>(20);
        for (int i=0; i<20; i++){
            p2pMap.Add(new List<int>(20));
            switch(i){
                case 0:{
                    p2pMap[i].Add(5);
                    break;}
                case 1:{
                    p2pMap[i].Add(6);
                    break;}
                case 2:{
                    p2pMap[i].Add(8);
                    break;}
                case 3:{
                    p2pMap[i].Add(10);
                    break;}
                case 4:{
                    p2pMap[i].Add(5);
                    break;}
                case 5:{
                    p2pMap[i].Add(0);
                    p2pMap[i].Add(4);
                    p2pMap[i].Add(13);
                    p2pMap[i].Add(19);
                    break;}
                case 6:{
                    p2pMap[i].Add(1);
                    p2pMap[i].Add(19);
                    p2pMap[i].Add(7);
                    break;}
                case 7:{
                    p2pMap[i].Add(6);
                    p2pMap[i].Add(15);
                    p2pMap[i].Add(8);
                    break;}
                case 8:{
                    p2pMap[i].Add(2);
                    p2pMap[i].Add(7);
                    p2pMap[i].Add(9);
                    break;}
                case 9:{
                    p2pMap[i].Add(8);
                    p2pMap[i].Add(17);
                    p2pMap[i].Add(10);
                    break;}
                case 10:{
                    p2pMap[i].Add(3);
                    p2pMap[i].Add(9);
                    p2pMap[i].Add(11);
                    break;}
                case 11:{
                    p2pMap[i].Add(10);
                    break;}
                case 12:{
                    p2pMap[i].Add(13);
                    break;}
                case 13:{
                    p2pMap[i].Add(5);
                    p2pMap[i].Add(12);
                    p2pMap[i].Add(14);
                    break;}
                case 14:{
                    p2pMap[i].Add(13);
                    p2pMap[i].Add(15);
                    break;}
                case 15:{
                    p2pMap[i].Add(7);
                    p2pMap[i].Add(14);
                    p2pMap[i].Add(16);
                    break;}
                case 16:{
                    p2pMap[i].Add(15);
                    p2pMap[i].Add(17);
                    break;}
                case 17:{
                    p2pMap[i].Add(9);
                    p2pMap[i].Add(16);
                    p2pMap[i].Add(18);
                    break;}
                case 18:{
                    p2pMap[i].Add(17);
                    break;}
                case 19:{
                    p2pMap[i].Add(5);
                    p2pMap[i].Add(6);
                    break;}
            }
        }
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)){
            SetCarGoal(0, 18);
            SetCarGoal(1, 12);
            SetCarGoal(2, 4);
            SetCarGoal(3, 15);
            SetCarGoal(4, 13);
            // SetCarGoal(5, 10);
            // SetCarGoal(6, 17);
        }
    }


    //银行家算法，进行进程模拟
    //如果成功，返回true，并且状态为2的车继续走（回到状态1）
    //否则返回false调用者不走
    public bool BankerSaver( int selfIndex ){
        int[] cannotMove = new int[pointList.Count];
        int[] cannotMoveCauseBy = new int[pointList.Count];

        int needToCombieNum = 0;
        for(int i=0; i < carCtrlList.Count; i++){
            if(carCtrlList[i].state != 0){
                needToCombieNum++;
            }
        }
        int[] a = new int[needToCombieNum];

        for(int i=0, j=0; i < carCtrlList.Count; i++){
            if(carCtrlList[i].state != 0){
                a[j] = i;
                j++;
            }
        }
        bool hasNextCombie = true;
        while(hasNextCombie){
            //按照 a 运行序列的 一次完整模拟
            SetInitCannotMove(ref cannotMove, ref cannotMoveCauseBy);

            if(cannotMove[ carCtrlList[selfIndex].holdTagNext ]>1){
                return false;//有人当前占用了自己的想要的位置，直接就不可以批准
            }

                
            bool canAllArriveGoal = true;
            int i;

            //如果在该车之前有车在等，那么这种排列是不允许的
            int selfTar = -1;
            for(int j=0;j<a.Length;j++){
                if(a[j]==selfIndex){
                    selfTar = j;
                }
            }
            for(int j=0;j<selfTar;j++){
                if(carCtrlList[a[j]].state == 2){
                    canAllArriveGoal = false;
                }
            }
            
            if(canAllArriveGoal)
                for( i=0;i<a.Length;i++){
                    if(cannotMove[ carCtrlList[a[i]].holdTagNext ] > 1 ){//
                        canAllArriveGoal = false;
                        break;
                    }
                    for(int j=0;j<carCtrlList[a[i]].pathList.Count;j++){
                        if(cannotMove[ carCtrlList[a[i]].pathList[j] ] > 0){
                            canAllArriveGoal = false;
                            break;
                        }
                    }             
                    
                    if(!canAllArriveGoal) { 
                        break;
                    }//按照a序列 模拟过程中，有车无法到达目的地，尝试下一次序列组合
                    //a 序列模拟中，a[i]车到达了目的地，更新其占用
                    if(carCtrlList[a[i]].state==1){
                        cannotMove[ carCtrlList[a[i]].holdTagNext ] -= 1;
                        if(carCtrlList[a[i]].pathList.Count == 0){
                            cannotMove[ carCtrlList[a[i]].holdTagNext ] += 1;
                        }else{
                            cannotMove[ carCtrlList[a[i]].pathList[carCtrlList[a[i]].pathList.Count-1] ] += 1;
                        }
                        
                    }else if(carCtrlList[a[i]].state==2){
                        cannotMove[ carCtrlList[a[i]].holdTagNow ] -= 1;
                        if(carCtrlList[a[i]].pathList.Count == 0){
                            cannotMove[ carCtrlList[a[i]].holdTagNext ] += 1;
                        }else{ 
                            cannotMove[ carCtrlList[a[i]].pathList[carCtrlList[a[i]].pathList.Count-1] ] += 1;
                        }
                    }
                }

            if(canAllArriveGoal) {
                return true;//当前状态是安全的，运行调用者获得资源
            }

            if(CombineUtil.nextCombie(ref a)==null){
                hasNextCombie = false;
            }
        }
        return false;
    }

    private void SetInitCannotMove(ref int[] cannotMove, ref int[] cannotMoveCauseBy ){
        for(int i=0;i<cannotMove.Length;i++){
            cannotMove[i]=0;
            cannotMoveCauseBy[i]=0;
        }
        for (int i=0; i<carList.Count; i++){
            if(carList[i].GetComponent<Car>().state==0 || carList[i].GetComponent<Car>().state==2){
                cannotMove[ carList[i].GetComponent<Car>().holdTagNow ] += 1;
                cannotMoveCauseBy[ carList[i].GetComponent<Car>().holdTagNow ] = i;
            }else{
                cannotMove[ carList[i].GetComponent<Car>().holdTagNext ] += 1;
                cannotMoveCauseBy[ carList[i].GetComponent<Car>().holdTagNext ] = i;
            }
        }
    }


    private int TellTime=0;

    public void TellWaitingCarToGo(){ 
        for(int i=0;i<carList.Count;i++){
            if(carList[i].GetComponent<Car>().state == 2)
                carList[i].GetComponent<Car>().TellCheckToGo();
            }
    }

    public void TellWaitingCarToGoIfAllWaiting(){
        bool isAllWaiting = true;
        if(TellTime > 100){
            return;
        }
        for(int i=0;i<carList.Count;i++){
            if(carCtrlList[i].state == 1){
                isAllWaiting = false;
            }
        }
        if(isAllWaiting){
            TellWaitingCarToGo();
        }
    }

    public void SetCarGoal(int car, int goalTag){
        Queue<spfaQItem> spfaQ = new Queue<spfaQItem>(100);
        List<spfaQItem> listPoint = new List<spfaQItem>(100);
        spfaQItem item;
        int[] arrived = new int[pointList.Count];
        if(carList[car].GetComponent<Car>().state == 0){
            item = new spfaQItem(carList[car].GetComponent<Car>().holdTagNow, -1);
            spfaQ.Enqueue(item);
            listPoint.Add(item);
            if(carList[car].GetComponent<Car>().holdTagNow == goalTag){
                return;
            }
            arrived[item.tagIndex] = 1;
        }else{
            item = new spfaQItem(carList[car].GetComponent<Car>().holdTagNext, -1);
            spfaQ.Enqueue(item);
            listPoint.Add(item);
            if(carList[car].GetComponent<Car>().holdTagNext == goalTag){
                return;
            }
            arrived[item.tagIndex] = 1;
        }
        while(spfaQ.Count!=0){
            spfaQItem h = spfaQ.Dequeue();
            for(int i=0; i<p2pMap[h.tagIndex].Count; i++){
                int t = p2pMap[h.tagIndex][i];
                if(arrived[t]!=1){// not arrived
                    arrived[t] = 1;
                    
                    item = new spfaQItem(t, h.tagIndex);
                    spfaQ.Enqueue(item);
                    listPoint.Add(item);
                    if(item.tagIndex == goalTag){
                        break;
                    }
                }
            }
            if(item.tagIndex == goalTag){
                    break;
            }
        }

        List<int> pathList = new List<int>(100);
        pathList.Add(item.tagIndex);
        while(item.preTag != -1){
            for(int i=0; i<listPoint.Count; i++){
                if(listPoint[i].tagIndex == item.preTag){
                    item = listPoint[i];
                    pathList.Add(item.tagIndex);
                    break;
                }
            }
        }
        pathList.Reverse();

        StringBuilder s = new StringBuilder();
        for(int i=0;i<pathList.Count;i++){
            s.Append("  "+pathList[i]+",  ");
        }
        Debug.Log(s);

        carList[car].GetComponent<Car>().SetPathList(pathList);
    }
    class spfaQItem{
        public int tagIndex;
        public int preTag;
        public spfaQItem(int _tagIndex, int _preTag){
            tagIndex = _tagIndex;
            preTag = _preTag;
        }
    }
}
