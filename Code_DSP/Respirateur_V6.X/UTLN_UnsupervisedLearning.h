/* 
 * File:   UTLN_UnsupervisedLearning.h
 * Author: sebas
 *
 * Created on 21 septembre 2020, 17:07
 */

#ifndef UTLN_UNSUPERVISEDLEARNING_H
#define	UTLN_UNSUPERVISEDLEARNING_H

#ifdef	__cplusplus
extern "C" {
#endif

#define DistanceMaximumConfigured           1.25
#define OffsetConfigured                    1
#define AlphaConfigured                     0.05    
    
#define NeuronsInputLayerC                  3
#define MaxQClustersC                       25
#define ActualNumberOfClustersC             1
#define NumberSamplesTrainingSetC           25
//Minimum number of samples by cluster
#define minimumNumberOfSamplesInCluster     4    
#define NumberOfSamplesForInitialization    6

    
typedef enum{
    First_Sample,
    Clusters_Comparison,        
}StateTraining;
    
    
void InitUnsupervisedKMeansModif(float DistanceMax, float alphaLPF, float clusterOffset);
int Training(float sample[]);
int AnomalyDetection(float sample[]);
int GetNumberOfClusters(void);
int GetClusterDimension(void);
float LPFKMeans(float xi, float yi_1, float alpha);
void ResetAnomaliesCounter(void);
#ifdef	__cplusplus
}
#endif

#endif	/* UTLN_UNSUPERVISEDLEARNING_H */

