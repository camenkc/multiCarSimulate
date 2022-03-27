using System.Text;
using UnityEngine;
static public class CombineUtil{
    static public int[] nextCombie(ref int[] a){
        if(a == null) return null;
        int len = a.Length;
        //是否是最后一个:降序
        bool isLowerList = true;
        if(len <= 1) return null;
        for(int i=1;i<len;i++){
            if(a[i] > a[i-1]){
                isLowerList = false;
                break;
            }
        }
        if(isLowerList) return null;


        //从右往左找到第一个比相邻右边数字小的
        int tar = 0;
        for(int i=len-2;i>=0;i--){
            if(a[i] < a[i+1]){
                tar = i;
                break;
            }
        }        

        //在该数字后的数字中找出比它大的数中最小的数 
        int min = 999999;
        int minTar = 0;
        for(int i=tar+1;i<len;i++){
            if(a[i]>a[tar]){
                if(a[i]<min){
                    min = a[i];
                    minTar = i;
                }
            }
        }
        int tmp = a[tar];
        a[tar] = a[minTar];
        a[minTar] = tmp;

        for(int i=tar+1,j=len-1;i<=j;i++,j--){
            tmp = a[i];
            a[i]=a[j];
            a[j]=tmp;
        }

        return a;
    }

    
} 