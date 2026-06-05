# 헤이 치즈! (Hey Cheese!)

<br>

> 감정을 배우고, 표현하고, 관계를 만들어가는 얼굴 인식 기반 인터랙티브 게임

<img src="https://github.com/limce106/HeyCheese/blob/main/readme/heycheese_1.png?raw=true" width="420"/>

<br>

[![YouTube](https://img.shields.io/badge/시연_영상_보기-FF0000?style=for-the-badge&logo=youtube&logoColor=white)](https://youtu.be/4RVUEoXP8Yw?si=6wZkuw21XVHzOw8i)

<br>

## 📊 게임 정보

|     항목     | 내용                              |
| :--------: | :------------------------------ |
|   **장르**   | 기능성 게임, 인터랙티브 스토리, 정서 교육 게임     |
|   **개발팀**  | EMO                             |
|   **플랫폼**  | Android                         |
| **대상 사용자** | 경계선 지능 아동 및 초등 저학년 아동           |
|  **개발 엔진** | Unity                           |
|  **핵심 기술** | 얼굴 인식, 감정 인식, AR 필터, TTS, 로컬 DB |
|  **개발 목적** | 감정 표현 및 사회적 관계 형성 지원            |

<br>

## 🎮 게임 소개

**헤이 치즈**!는 경계선 지능 아동과 초등 저학년 아동이 게임 속 캐릭터와 상호작용하며 감정을 이해하고 표현할 수 있도록 설계된 **얼굴 인식 기반 인터랙티브 기능성 게임**입니다.

사용자는 캐릭터 **치즈**와 **부기**를 만나 일상 속 상황을 바탕으로 한 에피소드를 플레이하고, 선택지, 미니게임, 표정 촬영, AR 필터 카메라를 통해 자신의 감정을 자연스럽게 표현합니다.

게임은 정답과 실패를 강하게 구분하기보다, 사용자의 선택과 반응을 긍정적으로 해석하고 지지하는 피드백을 제공하여 정서적 안정감과 자기 효능감을 높이는 것을 목표로 합니다.

<br>

## ⚙️ Technical Stack

<div>
<img src="https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white"/>
<img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white"/>
<img src="https://img.shields.io/badge/Android-3DDC84?style=flat-square&logo=android&logoColor=white"/>
<img src="https://img.shields.io/badge/ARCore-4285F4?style=flat-square&logo=google&logoColor=white"/>
<img src="https://img.shields.io/badge/Google_Cloud_Vision-4285F4?style=flat-square&logo=googlecloud&logoColor=white"/>
<img src="https://img.shields.io/badge/SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white"/>
</div>

<br>

* **Game Engine**: Unity
* **Language**: C#
* **Platform**: Android
* **Face Tracking / AR**: ARCore
* **Emotion Recognition**: Google Cloud Vision API
* **Voice Feedback**: Google TTS
* **Data Management**: SQLite, Google Spreadsheet CSV
* **Local Storage**: 감정 사진 및 플레이 데이터 저장

<br>

## 📱 Screenshots

| ![Screenshot1](https://raw.githubusercontent.com/limce106/HeyCheese/main/readme/heycheese_2.png) | ![Screenshot2](https://raw.githubusercontent.com/limce106/HeyCheese/main/readme/heycheese_3.png) | ![Screenshot3](https://raw.githubusercontent.com/limce106/HeyCheese/main/readme/heycheese_4.png) |
| -------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| ![Screenshot4](https://raw.githubusercontent.com/limce106/HeyCheese/main/readme/heycheese_5.png) | ![Screenshot5](https://raw.githubusercontent.com/limce106/HeyCheese/main/readme/heycheese_6.png) | ![Screenshot6](https://raw.githubusercontent.com/limce106/HeyCheese/main/readme/heycheese_7.png) |

<br>

## 💡 개발 배경

### 경계선 지능 아동의 정서 및 사회 관계 지원 필요성

경계선 지능 아동은 지적장애로 분류되지는 않지만, 일반 아동에 비해 학습 속도와 사회적 적응에서 어려움을 겪을 수 있습니다. 특히 일반 학급과 특수 학급 사이에서 명확한 소속감을 느끼기 어렵고, 반복적인 실패 경험과 부정적 피드백은 정서적 소외감, 낮은 자기 효능감, 또래 관계 부적응으로 이어질 수 있습니다.

하지만 기존의 지원은 주로 인지 학습이나 언어 훈련에 집중되어 있어, 감정 표현과 사회적 관계 형성을 자연스럽게 연습할 수 있는 도구는 상대적으로 부족합니다.

<br>

### 개발 목적

**헤이 치즈**!는 경계선 지능 아동이 게임 속 안전한 환경에서 감정을 표현하고, 타인의 감정을 이해하며, 긍정적인 관계 경험을 반복할 수 있도록 설계되었습니다.

1. **다양한 표정과 감정 표현 학습**
2. **실제 상황 기반의 사회적 반응 연습**
3. **의사소통에 대한 자신감 형성**
4. **긍정적 피드백을 통한 정서적 안정감 제공**

<br>

## ✨ 게임 특징

### 1) 얼굴 인식 기반 감정 표현 시스템

<img src="https://github.com/limce106/HeyCheese/blob/main/readme/heycheese_8.png?raw=true" width="420"/>

스토리 진행 중 사용자는 카메라를 통해 자신의 표정을 촬영합니다.
게임은 Google Cloud Vision API를 활용하여 표정 속 감정 정보를 인식하고, 이를 스토리 상호작용에 반영합니다.

이를 통해 사용자는 단순히 선택지를 고르는 것이 아니라, 자신의 얼굴과 표정을 활용해 감정을 직접 표현하는 경험을 하게 됩니다.

<br>

### 2) 에피소드 기반 인터랙티브 스토리

<img src="https://github.com/limce106/HeyCheese/blob/main/readme/heycheese_9.png?raw=true" width="420"/>

게임은 초등 저학년 아동이 일상에서 경험할 수 있는 상황을 바탕으로 한 에피소드 구조로 구성됩니다.

각 에피소드에서는 대화 읽기, 선택지 고르기, 표정 촬영, 미니게임, 감정 선택 등 다양한 상호작용이 이어지며, 사용자는 캐릭터와 관계를 맺으며 자연스럽게 사회적 상황을 이해합니다.

<br>

### 3) AR 필터 카메라: 치즈 한 컷

<img src="https://github.com/limce106/HeyCheese/blob/main/readme/heycheese_10.png?raw=true" width="420"/>

**치즈 한 컷**은 AR 필터와 프레임을 활용해 자유롭게 사진을 촬영할 수 있는 기능입니다.

사용자는 에피소드를 클리어하거나 히든 미션을 달성하며 새로운 필터와 프레임을 획득할 수 있고, 이를 통해 반복적인 표정 표현과 자기 표현 활동에 자연스럽게 참여하게 됩니다.

<br>

### 4) 감정 보관함

<img src="https://github.com/limce106/HeyCheese/blob/main/readme/heycheese_11.png?raw=true" width="420"/>

**감정 보관함**은 게임 중 촬영한 사진과 감정 데이터를 저장하고 다시 확인할 수 있는 앨범형 기능입니다.

사진에는 촬영 시점, 에피소드, 감정 분류 결과, 선택지 정보 등이 함께 저장되며, 사용자는 자신의 감정 표현 경험을 다시 돌아보며 자기 인식과 정서 표현 능력을 확장할 수 있습니다.

<br>

## 🎬 게임 구조

### 전체 플레이 흐름

```
메인 화면
  ├─ 치즈 한 컷
  │   ├─ AR 필터 선택
  │   ├─ 프레임 선택
  │   └─ 사진 촬영 및 저장
  │
  ├─ 스토리 게임
  │   ├─ 에피소드 선택
  │   ├─ 대사 진행
  │   ├─ 선택지 상호작용
  │   ├─ 감정 검출 카메라
  │   ├─ 미니게임
  │   └─ 오늘 기분 선택
  │
  ├─ 감정 보관함
  │   ├─ 사진 목록
  │   ├─ 사진 상세 보기
  │   └─ 저장 및 삭제
  │
  └─ 설정
      ├─ 사운드 조절
      ├─ 보유 가이드
      └─ 제작자 정보
```

<br>

### 주요 콘텐츠 구성

**메인 기능**

1. 치즈 한 컷
2. 스토리 게임
3. 감정 보관함
4. 설정

**스토리 게임**

* 총 4개의 에피소드 기반 구성
* 일상적 사회 상황 중심의 시나리오
* 감정 표현, 선택지, 미니게임, 사진 촬영이 결합된 진행 방식

**상호작용 요소**

* 공감 대화 선택지
* 표정 촬영 및 감정 인식
* AR 필터 카메라
* 미니게임
* 오늘 기분 선택
* 감정 사진 저장 및 회고

<br>

## 🧩 시스템 구조

<img src="https://github.com/limce106/HeyCheese/blob/main/readme/heycheese_system.png?raw=true" width="720"/>

<br>

본 프로젝트는 Unity를 중심으로 여러 외부 기술을 연동하여 구현되었습니다.

* **Unity**: 게임 로직, UI, 스토리 진행, 미니게임 구현
* **ARCore**: 얼굴 추적 및 AR 필터 적용
* **Google Cloud Vision API**: 표정 기반 감정 인식
* **Google TTS**: 게임 내 대사 음성 출력
* **Google Spreadsheet**: 스토리 텍스트 및 리소스 데이터 관리
* **SQLite**: 감정 사진, 선택지, 감정 분류 결과 등 로컬 데이터 저장

<br>

## 🧪 파일럿 테스트

본 프로젝트는 초기 사용성 및 정서 반응 검증을 위해 일반 초등학생 30명을 대상으로 파일럿 테스트를 진행했습니다.

테스트는 게임 체험, 설문 조사, 자유 소감 작성의 순서로 진행되었으며, 문장 이해도, 게임 난이도, 사회적 학습 가능성, 몰입도, 정서적 만족도, 미디어 요소 만족도, 전반적 흥미도를 평가했습니다.

<br>

### 주요 결과

| 평가 항목                   | 긍정 응답 |
| :---------------------- | :---: |
| 게임 속 말이 이해하기 쉬웠나요?      |  96%  |
| 미니게임이 어렵지 않았나요?         |  100% |
| 친구와 더 잘 지내는 법을 배웠나요?    |  100% |
| 이 게임을 계속 플레이하고 싶나요?     |  93%  |
| 게임을 하고 나서 행복한 기분이 들었나요? |  96%  |
| 게임의 그림과 소리가 마음에 들었나요?   |  80%  |
| 게임이 재미있었나요?             |  90%  |

<br>

테스트 결과, 사용자는 감정 표현 활동과 사진 촬영 기능에 높은 흥미를 보였으며, 게임의 이해도와 정서적 만족도에서도 긍정적인 반응을 나타냈습니다.

<br>

## 🏆 핵심 성과

* **경계선 지능 아동의 정서 및 사회 관계 형성 지원을 위한 기능성 게임 설계**
* **얼굴 인식, AR 필터, TTS, 로컬 DB를 결합한 Unity 기반 Android 게임 구현**
* **일반 초등학생 30명 대상 파일럿 테스트 진행**
* **JCCT 논문 게재**

  * *Facial Recognition-Based Interactive Game to Support Emotional and Social Relationship Formation in Borderline Intellectual Functioning Children*
  * The Journal of the Convergence on Culture Technology, Vol. 11, No. 6, pp. 321-327, 2025
  * DOI: http://dx.doi.org/10.17703/JCCT.2025.11.6.321

<br>

## 👥 Team EMO

|    이름   | 역할                                                      |
| :-----: | :------------------------------------------------------ |
| **조수민** | 기획, 시나리오, UI/UX                                         |
| **서혜령** | Unity Client Development, System Design                 |
| **오지은** | Unity Client Development, Mini-game Development         |
| **임채은** | Unity Client Development, Database / Interaction System |
| **엄성용** | Project Advisor                                         |

<br>

## 📄 Publication

**경계선 지능 아동의 정서 및 사회 관계 형성 지원을 위한 얼굴 인식 기반 인터랙티브 게임**
*Facial Recognition-Based Interactive Game to Support Emotional and Social Relationship Formation in Borderline Intellectual Functioning Children*

* Journal: The Journal of the Convergence on Culture Technology (JCCT)
* Volume: 11
* Issue: 6
* Pages: 321-327
* Published: 2025.11.30
* DOI: http://dx.doi.org/10.17703/JCCT.2025.11.6.321

---

<div align="center">

**Made with 💙 by Team EMO**

감정을 표현하고, 관계를 배우는 따뜻한 인터랙티브 게임

</div>
