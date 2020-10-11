#include "UTLN_UnsupervisedLearning.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include "Define.h"


int QneuronsInputLayer = NeuronsInputLayerC;
int maxQClusters = MaxQClustersC;
int actualNumberOfClusters;// = ActualNumberOfClustersC;
int numberSamplesTrainingSet = NumberSamplesTrainingSetC;
int stateKMM = 0;

//Number of clusters after eliminating the ones with few samples
int finalNumberofClusters = 0;
//Number of clusters after eliminated beacause they have few samples
int numberofClustersDeleted = 0;


volatile StateTraining stateTraining = First_Sample;

int iterations = 0;

//Matrix of Clusters
float Clusters[MaxQClustersC][NeuronsInputLayerC] = {0.0f};

//Matrix of Dispersions
float Dispersions[MaxQClustersC][NeuronsInputLayerC] = {0.0f};

//Initial value of Dispersions
float SamplesForInitialization[NumberOfSamplesForInitialization][NeuronsInputLayerC] = {0.0f};
float DispersionsInitialValue[NeuronsInputLayerC] = {0.0f};

//Matrix of samples counter on each Cluster
float SamplesCounter[MaxQClustersC] = {0.0f};

//Sample i
float Sample_i[NeuronsInputLayerC] = {0.0f};
//Sample i-1
float Sample_i_1[NeuronsInputLayerC] = {0.0f};

float AlphaLPF;

//Clusters to print
float ClusterToPrint[NeuronsInputLayerC] = {0.0f};
//int neuronsInputLayer = 0;
//int maxQClusters = 0;
//int actualNumberOfClusters = 0;
//int numberSamplesTrainingSet = 0;
//int state = 0;

float distanceMax = 2.0f;
float offSetCluster = 0.05f;

int AnomaliesCounter = 0;

bool increaseNumberOfCluster = false;

//void InitUnsupervisedKMeansModif(int NeuronsInputLayer, int MaxQClusters,
//        int NumberSamplesTrainingSet, float DistanceMax, float alphaLPF)
void InitUnsupervisedKMeansModif(float DistanceMax, float alphaLPF, float clusterOffset)
{
    //Whole the variables for each cluster
    //QneuronsInputLayer = NeuronsInputLayer;
    //Maximum quantity of clusters
    //maxQClusters = MaxQClusters;
    //Number of samples for the training set
    //numberSamplesTrainingSet = NumberSamplesTrainingSet;
    
    //Distance max between the points and the cluster
    distanceMax = DistanceMax;
    //Initial Number of clusters
    actualNumberOfClusters = 0;
    //Number of clusters after eliminating the ones with few samples
    finalNumberofClusters = 0;
    //Number of clusters after eliminated beacause they have few samples
    numberofClustersDeleted = 0;
    int i=0;
    for(i=0;i<MaxQClustersC;i++)
        SamplesCounter[i] = 0.0f;
    
    //alpha low pass filter
    AlphaLPF = alphaLPF;

    offSetCluster = clusterOffset;
    
    increaseNumberOfCluster = false;
    
    stateTraining = First_Sample;
    
    iterations = 0;
    
    AnomaliesCounter = 0;
}

int GetNumberOfClusters(void){
    return actualNumberOfClusters;
}

//Training function without dispersions
//Training will return:
//0 if training is not finished yet
//1 if training is finished 
//5 if training does not converge
//int Training(float sample[]){
//
//    float dist = 0;
//
//    switch(stateTraining){
//        case First_Sample :
//        {
//            //The first sample is asigned as the first cluster centroid 
//            int i;
//            for (i = 0; i < QneuronsInputLayer; i++)                
//                Clusters[0][i] = sample[i];
//            //At this moment we have only one Cluster
//            actualNumberOfClusters = 1;
//
//            iterations++;
//            stateTraining = Clusters_Comparison;
//            break;
//        }
//        case Clusters_Comparison :
//        {
//            //Compare with each cluster
//            int i;
//            for(i = 0; i <actualNumberOfClusters; i++)
//            {
//                dist = 0;
//                int j;
//                for (j = 0; j < QneuronsInputLayer; j++)
//                {
//                    if(Clusters[i][j] > offSetCluster)
//                    //calculate distance between the sample and the cluster
//                    dist += powf((Clusters[i][j] - sample[j])/Clusters[i][j], 2.0f);
//                    else
//                    dist += powf((Clusters[i][j] - sample[j])/(Clusters[i][j]+offSetCluster), 2.0f);                        
//                }
//
//                dist = sqrtf(dist);
//                if (dist < distanceMax)
//                {
//                    //If dist is lower than distanceMax, this sample belong to the Actual cluster
//                    //This cluster is rectified with the average of each dimensions                    
//                    for (j = 0; j < QneuronsInputLayer; j++)
//                    {
//                        //Clusters[jjjC][jjj] = (sample[jjj] + Clusters[jjjC][jjj]) / 2;
//                        Clusters[i][j] = LPFKMeans(sample[j],Clusters[i][j],AlphaLPF);
//                    }
//                    iterations++;
//                    if(iterations < numberSamplesTrainingSet)
//                        return 0;
//                    else
//                        return 1;
//                }
//            }
//            //if all the clusters had been compared
//            //New cluster had been detected
//            for (i = 0; i < QneuronsInputLayer; i++)
//            {
//                //Save sample in a new cluster
//                Clusters[actualNumberOfClusters][i] = sample[i];
//            }
//            actualNumberOfClusters++;
//            #if COMUSBDEBUG
//                PrintClusterUSB(sample, QneuronsInputLayer,
//                            actualNumberOfClusters+1);
//            #endif
//            if(actualNumberOfClusters == maxQClusters)
//                return 5;
//            iterations++;
//            break;
//        }
//    }
//
//    if(iterations < numberSamplesTrainingSet)
//        return 0;
//    else
//        return 1;
//}
#if 0
//Training function with dispersions
//Training will return:
//0 if training is not finished yet
//1 if training is finished 
//5 if training does not converge
int Training(float sample[]){

    float dist = 0;

    switch(stateTraining){
        case First_Sample :
        {
            //The first sample is asigned as the first cluster centroid 
            int i;
            for (i = 0; i < QneuronsInputLayer; i++){
                //The first cluster is initialized with the first sample of each feature
                Clusters[0][i] = sample[i];
                //The first cluster is initialized with the half of the first sample of each feature
//                if(sample[i]>0.01)
                    Dispersions[0][i] = sample[i]/2.0f;
//                else
//                    Dispersions[0][i] = 0.01f;
            }                
            
            //As it is the first sample we clear the counters of samples for each cluster 
            for(i=0;i<MaxQClustersC;i++)
                SamplesCounter[i] = 0.0f;
            
            //The actual number of samples on the first cluster is one 
            SamplesCounter[0] = 1;
            
            //At this moment we have only one Cluster
            actualNumberOfClusters = 1;

            //Iterations incremented for after the total defined passing to detection mode
            iterations++;

            //Next state 
            stateTraining = Clusters_Comparison;
            break;
        }
        case Clusters_Comparison :
        {
            //Compare with each cluster
            int i;
            for(i = 0; i <actualNumberOfClusters; i++)
            {
                dist = 0;
                int j;
                for (j = 0; j < QneuronsInputLayer; j++)
                {
                    if(sample[0] + sample[1] + sample[2] > 0.01f){
                        if(Dispersions[i][j] > offSetCluster)
                        //calculate distance between the sample and the cluster
                        dist += powf((Clusters[i][j] - sample[j])/(Dispersions[i][j]), 2.0f);
                        else
                        dist += powf((Clusters[i][j] - sample[j])/(Dispersions[i][j]+offSetCluster), 2.0f);                        
                    }
                    else
                        dist = distanceMax/2;
                }

                dist = sqrtf(dist);
                if (dist < distanceMax)
                {
                    //If dist is lower than distanceMax, this sample belong to the Actual cluster
                    //This cluster and the dispersions are rectified with the samples features                    
                    for (j = 0; j < QneuronsInputLayer; j++)
                    {
                        //Clusters[jjjC][jjj] = (sample[jjj] + Clusters[jjjC][jjj]) / 2;
                        Clusters[i][j] = LPFKMeans(sample[j],Clusters[i][j],AlphaLPF);
                        Dispersions[i][j] = LPFKMeans(10.0f*abs(sample[j]-Clusters[i][j]),Dispersions[i][j],AlphaLPF);
                    }
                    //Increment of the samples of the cluster
                    SamplesCounter[i]++;

                    //Iterations incremented for after the total defined passing to detection mode
                    iterations++;
                    if(iterations < numberSamplesTrainingSet)
                        return 0;//Training mode
                    else{
                        //Delete the cluster with less than X samples
                        bool allClustersVerified = false;
                        int j=0;
                        while(!allClustersVerified){
                            if(SamplesCounter[j]<minimumNumberOfSamplesInCluster){
                                int k, l;
                                for(k = j; k <actualNumberOfClusters-1; k++)
                                {
                                    for (l = 0; l < QneuronsInputLayer; l++)
                                    {
                                        Clusters[k][l] = Clusters[k+1][l];
                                        Dispersions[k][l] = Dispersions[k+1][l];
                                    }
                                    SamplesCounter[k] = SamplesCounter[k+1];
                                }
                                numberofClustersDeleted++;
                            }
                            else{
                                finalNumberofClusters++;
                                j++;
                            }

                            if(numberofClustersDeleted + finalNumberofClusters == actualNumberOfClusters)
                                allClustersVerified = true;
                        }
                        actualNumberOfClusters = finalNumberofClusters;
                        return 1;//Detection mode, training finished                         
                    }
                }
            }
            
            //if all the clusters had been compared
            //New cluster had been detected
            for (i = 0; i < QneuronsInputLayer; i++)
            {
                //Save sample in a new cluster
                Clusters[actualNumberOfClusters][i] = sample[i];
                //Initialize dispersion of the new cluster
                Dispersions[actualNumberOfClusters][i] = sample[i]/2;
            }
            
            //The actual number of samples on the first cluster is one 
            SamplesCounter[actualNumberOfClusters] = 1;
            
            actualNumberOfClusters++;
            #if COMUSBDEBUG
                PrintClusterUSB(sample, QneuronsInputLayer,
                            actualNumberOfClusters+1);
            #endif
            if(actualNumberOfClusters == maxQClusters)
                return 5;
            iterations++;
            break;
        }
    }

    if(iterations < numberSamplesTrainingSet)
        return 0;
    else
        return 1;
}
#endif

//Training function with dispersions
//Training will return:
//0 if training is not finished yet
//1 if training is finished 
//5 if training does not converge
int Training(float sample[]){

    float dist = 0;

    switch(stateTraining){        
        case First_Sample :
        {//In the first state the first N samples are used for initializing the dispersions 

        //The first N samples are saved     
            int i;
            for (i = 0; i < QneuronsInputLayer; i++){
                SamplesForInitialization[iterations][i] = sample[i];
                //The first cluster is initialized with the first sample of each feature
                //Clusters[0][i] = sample[i];
                //The first cluster is initialized with the half of the first sample of each feature
            }                
            
            //Iterations incremented for after the total defined passing to detection mode
            iterations++;

            //Next state
            if(iterations >= NumberOfSamplesForInitialization){

                //When we had received the first N samples 
                //We calculate the average distance between the points 
                float distBetweenSamples[NeuronsInputLayerC] = {0};                                
                for (i = 0; i < NumberOfSamplesForInitialization; i++){
                    int j=0;
                    for (j = i; j < NumberOfSamplesForInitialization; j++){
                        if(i!=j){
                            int k=0;
                            for (k = 0; k < QneuronsInputLayer; k++)
                                distBetweenSamples[k] += fabsf(SamplesForInitialization[i][k] - SamplesForInitialization[j][k]);
                        }                    
                    }                    
                }

                float n = NumberOfSamplesForInitialization;
                for (i = 0; i < QneuronsInputLayer; i++)
                    DispersionsInitialValue[i] = (distBetweenSamples[i]/((n-1)*n/2))/2;
                
                //The first cluster is initialized with the last sample
                for (i = 0; i < QneuronsInputLayer; i++){
                    Clusters[0][i] = sample[i];
                    Dispersions[0][i] = DispersionsInitialValue[i];
                }
                
                //The actual number of samples on the first cluster is one 
                SamplesCounter[0] = 1;

                //At this moment we have only one Cluster
                actualNumberOfClusters = 1;
                
                //As it is the first sample we clear the counters of samples for each cluster 
                for(i=0;i<MaxQClustersC;i++)
                    SamplesCounter[i] = 0.0f;
                
                stateTraining = Clusters_Comparison;
            }

            break;
        }
        case Clusters_Comparison :
        {
            //Compare with each cluster
            int i;
            for(i = 0; i <actualNumberOfClusters; i++)
            {
                dist = 0;
                int j;
                for (j = 0; j < QneuronsInputLayer; j++)
                {
                    if(sample[0] + sample[1] + sample[2] > 0.01f){
                        if(Dispersions[i][j] > offSetCluster)
                        //calculate distance between the sample and the cluster
                        dist += powf((Clusters[i][j] - sample[j])/(Dispersions[i][j]), 2.0f);
                        else
                        dist += powf((Clusters[i][j] - sample[j])/(Dispersions[i][j]+offSetCluster), 2.0f);                        
                    }
                    else
                        dist = distanceMax/2;
                }

                dist = sqrtf(dist);
                if (dist < distanceMax)
                {
                    //If dist is lower than distanceMax, this sample belong to the Actual cluster
                    //This cluster and the dispersions are rectified with the samples features                    
                    for (j = 0; j < QneuronsInputLayer; j++)
                    {
                        //Clusters[jjjC][jjj] = (sample[jjj] + Clusters[jjjC][jjj]) / 2;
                        Clusters[i][j] = LPFKMeans(sample[j],Clusters[i][j],AlphaLPF);
                        Dispersions[i][j] = LPFKMeans(10.0f*fabsf(sample[j]-Clusters[i][j]),Dispersions[i][j],AlphaLPF);
                    }
                    //Increment of the samples of the cluster
                    SamplesCounter[i]++;

                    //Iterations incremented for after the total defined passing to detection mode
                    iterations++;
                    if(iterations < numberSamplesTrainingSet)
                        return 0;//Training mode
                    else{
                        //Delete the cluster with less than X samples
                        bool allClustersVerified = false;
                        int j=0;
                        while(!allClustersVerified){
                            if(SamplesCounter[j]<minimumNumberOfSamplesInCluster){
                                int k, l;
                                for(k = j; k <actualNumberOfClusters-1; k++)
                                {
                                    for (l = 0; l < QneuronsInputLayer; l++)
                                    {
                                        Clusters[k][l] = Clusters[k+1][l];
                                        Dispersions[k][l] = Dispersions[k+1][l];
                                    }
                                    SamplesCounter[k] = SamplesCounter[k+1];
                                }
                                numberofClustersDeleted++;
                            }
                            else{
                                finalNumberofClusters++;
                                j++;
                            }

                            if(numberofClustersDeleted + finalNumberofClusters == actualNumberOfClusters)
                                allClustersVerified = true;
                        }
                        actualNumberOfClusters = finalNumberofClusters;
                        return 1;//Detection mode, training finished                         
                    }
                }
            }
            
            //if all the clusters had been compared
            //New cluster had been detected
            for (i = 0; i < QneuronsInputLayer; i++)
            {
                //Save sample in a new cluster
                Clusters[actualNumberOfClusters][i] = sample[i];
                //Initialize dispersion of the new cluster
                Dispersions[actualNumberOfClusters][i] = DispersionsInitialValue[i];
                //Dispersions[actualNumberOfClusters][i] = sample[i]/2;
            }
            
            //The actual number of samples on the first cluster is one 
            SamplesCounter[actualNumberOfClusters] = 1;
            
            actualNumberOfClusters++;
            #if COMUSBDEBUG
                PrintClusterUSB(sample, QneuronsInputLayer,
                            actualNumberOfClusters+1);
            #endif
            if(actualNumberOfClusters == maxQClusters)
                return 5;
            iterations++;
            break;
        }
    }

    if(iterations < numberSamplesTrainingSet)
        return 0;
    else
        return 1;
}





//Anomaly detection without dispersion
//int AnomalyDetection(float sample[]){
//
//    float dist = 0;
//
//    //Compare with each cluster
//    int i;
//    for(i = 0; i <actualNumberOfClusters; i++)
//    {
//        dist = 0;
//        int j;
//        for (j = 0; j < QneuronsInputLayer; j++)
//        {
//            if(Clusters[i][j] > offSetCluster)
//            //calculate distance between the sample and the cluster
//            dist += powf((Clusters[i][j] - sample[j])/Clusters[i][j], 2.0f);
//            else
//            dist += powf((Clusters[i][j] - sample[j])/(Clusters[i][j]+offSetCluster), 2.0f);                        
//        }
//
//        dist = sqrtf(dist);
//        if (dist < distanceMax)
//        {
//            //If dist is lower than distanceMax, this sample belong to the Actual cluster
//            //This cluster is rectified with the average of each dimensions                    
//            for (j = 0; j < QneuronsInputLayer; j++)
//            {
//                //Clusters[jjjC][jjj] = (sample[jjj] + Clusters[jjjC][jjj]) / 2;
//                Clusters[i][j] = LPFKMeans(sample[j],Clusters[i][j],AlphaLPF);
//            }
//            return AnomaliesCounter;
//        }
//    }
//
//    //if all the clusters had been compared
//    AnomaliesCounter++;
//    //Anomaly detected!
//    return AnomaliesCounter;
//}

//Anomaly detection with dispersion
int AnomalyDetection(float sample[]){

    float dist = 0;

    //Compare with each cluster
    int i;
    for(i = 0; i <actualNumberOfClusters; i++)
    {
        dist = 0;
        int j;
        for (j = 0; j < QneuronsInputLayer; j++)
        {
            if(sample[0] + sample[1] + sample[2] > 0.01f){
//            if(Clusters[i][j] > offSetCluster)
//            //calculate distance between the sample and the cluster
//            dist += powf((Clusters[i][j] - sample[j])/Clusters[i][j], 2.0f);
//            else
//            dist += powf((Clusters[i][j] - sample[j])/(Clusters[i][j]+offSetCluster), 2.0f);                        
                if(Dispersions[i][j] > offSetCluster)
                //calculate distance between the sample and the cluster
                dist += powf((Clusters[i][j] - sample[j])/(Dispersions[i][j]), 2.0f);
                else
                dist += powf((Clusters[i][j] - sample[j])/(Dispersions[i][j]+offSetCluster), 2.0f);                                    
            }
            else
                dist = distanceMax/2;
        }

        dist = sqrtf(dist);
        if (dist < distanceMax)
        {
            //If dist is lower than distanceMax, this sample belong to the Actual cluster
            //This cluster is rectified with the average of each dimensions                    
            for (j = 0; j < QneuronsInputLayer; j++)
            {
                //Clusters[jjjC][jjj] = (sample[jjj] + Clusters[jjjC][jjj]) / 2;
                Clusters[i][j] = LPFKMeans(sample[j],Clusters[i][j],AlphaLPF);
                Dispersions[i][j] = LPFKMeans(10.0f*fabsf(sample[j]-Clusters[i][j]),Dispersions[i][j],4*AlphaLPF);
            }
            //Increment of the samples of the cluster
            SamplesCounter[i]++;
            
            return AnomaliesCounter;
        }
    }

    //if all the clusters had been compared
    AnomaliesCounter++;
    //Anomaly detected!
    return AnomaliesCounter;
}


//Anomaly detection function
//After training function returns 1 we could use this one
//it will return:
//                  0 if the sample belongs to some cluster 
//                  1 if the sample is an anomaly
//int AnomalyDetection(float sample[]){
//
//    float dist = 0;
//    int i;
//    int j;
//    //Compare with each cluster
//    for(i = 0; i <actualNumberOfClusters; i++)
//    {
//        dist = 0;
//        for (j = 0; j < QneuronsInputLayer; j++)
//        {
//            //calculate distance between the sample and the cluster
//            //dist += Math.Pow(Clusters[jjjC, jjj], 2) + Math.Pow(Sample_i[jjj], 2);
//            dist += powf((Clusters[i][j] - sample[j])/Clusters[i][j], 2.0f);
//        }
//        dist = sqrtf(dist);
//        if (dist < distanceMax)
//        {
//            //If dist is lower than distanceMax, this sample belong to the Actual cluster
//            //This cluster is rectified with the average of each dimensions
//            for (j = 0; j < QneuronsInputLayer; j++)
//            {
//                //Clusters[jjjC][jjj] = (sample[jjj] + Clusters[jjjC][jjj]) / 2;
//                Clusters[i][j] = LPFKMeans(Sample_i[j],Clusters[i][j],AlphaLPF);
//            }
//            return AnomaliesCounter;
//        }
//        else
//        {
//            //if all the clusters had been compared
//            if(i == (actualNumberOfClusters - 1))
//            {
//                AnomaliesCounter++;
//                //Anomaly detected!
//                return AnomaliesCounter;
//            }
//        }
//    }
//    return 0;
//
//}

//Low pass filter function to modify the clusters centroids 
float LPFKMeans(float xi, float yi_1, float alpha)
{
    float yi = (1 - alpha) * yi_1 + alpha * xi;
    if(yi>0.01f)
        return yi;
    else
        return 0.01f;
}

int GetClusterDimension(void){
    return NeuronsInputLayerC;
}

void ResetAnomaliesCounter(void){
    AnomaliesCounter=0;
}