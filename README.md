# 💰 사회초년생의 자산 관리 시뮬레이션 (Asset Management Sim)

![Game_Start_Screen](https://github.com/user-attachments/assets/1f4cb485-1f6b-400b-bfac-5070ea29dd05)

> **"10개월의 시간, 당신은 얼마나 많은 자산을 모을 수 있습니까?"**
> 사회초년생이 되어 월급 관리, 자기계발, 금융 활동을 통해 최고의 자산가에 도전해보세요!

---

## 📝 게임 소개 (Introduction)
본 게임은 이제 막 사회에 첫발을 내딛은 **사회초년생의 현실적인 경제 활동**을 담은 2D 시뮬레이션 게임입니다. 플레이어는 10개월이라는 한정된 시간 동안 매달 지급되는 월급을 효율적으로 관리하고, 자기계발을 통해 총자산을 올리며, 예기치 못한 사고와 스트레스에 대처해야 합니다. 단순한 저축을 넘어 대출과 적금 등 금융 시스템을 활용하여 최적의 자산을 모으는 것이 목표입니다.

## ✨ 주요 특징 (Key Features)
* **성장형 시스템**: 아카데미 교육을 통해 직무 레벨을 올리고 월급을 인상시키는 '자기계발' 요소 반영.
* **현실적인 금융 로직**: 정기적금의 강제 저축 기능과 대출 이자 시스템을 통한 전략적 자산 관리.
* **리스크 관리**: 무작위로 발생하는 교통사고와 매달 누적되는 스트레스 수치를 관리하는 긴장감 있는 플레이.
* **멀티 엔딩**: 최종 자산 상태뿐만 아니라 파산, 건강 악화(스트레스 오버) 등 플레이 결과에 따른 다양한 결말.

---

## 🎮 게임 조작 방법 (Controls)

* **이동**: 키보드 방향키 ($\uparrow, \downarrow, \leftarrow, \rightarrow$)를 사용하여 캐릭터를 조작합니다.
* **상호작용**: 각 건물의 입구(노란색 화살표)로 이동하면 전용 UI 패널이 활성화됩니다.
* **선택**: 마우스 클릭을 통해 모든 메뉴 및 퀴즈 정답을 선택합니다.

---

## 🏢 주요 장소 및 기능 (Locations)

### 전체 맵 구조
![Full_Map](https://github.com/user-attachments/assets/6e649853-f1db-4c29-adb2-0df135fe0420)

### 🏫 아카데미 (Academy)
![Office_Panel](https://github.com/user-attachments/assets/d57e417c-2f22-4c9c-ac10-56d1a1814cdd)
* **직무 교육**: 최대 **Lv.10**까지 상승 가능하며 레벨이 높을수록 월급이 인상됩니다.
* **월급 인상**: 직무 레벨 **1 상승당 월급 20만 원 인상** (최대 200만 원 추가 지급)
* **실무 강의 수강**: 비용 **15만 원** / 직무 레벨 **+1** / 스트레스 **+10**
* **실무 참여하기**: 비용 **100만 원** / 직무 레벨 **+3** / 스트레스 **+40**
* ※ 교육은 한 달에 한 번만 이용 가능합니다.

### 🏦 은행 (Bank)
![Bank_Panel](https://github.com/user-attachments/assets/247531ad-8e4e-4551-bd7c-0a0755438eaa)
* **대출 실행**: 최대 **200만 원**까지 가능 (클릭당 **50만 원** 대출).
* **대출 상환**: 클릭당 **50만 원**씩 원금 상환 가능.
* **정기적금**: 가입 시 매달 **50만 원**이 자동이체되어 저축됩니다.
* ※ 대출 미상환 시 매달 **대출 이자**가 발생하므로 빠른 상환이 고득점에 유리합니다.

### 🏠 나의 집 (Home)
![Home_Inside](https://github.com/user-attachments/assets/e48c532f-022e-4695-87e9-1d29c17296db)
* **다음 달로 이동**: 집에서 한 달을 마무리하고 **월급 명세서**를 수령합니다.
* ※ 다음 달로 이동할 때마다 업무 부담으로 인해 **스트레스가 40씩 증가**합니다.

### 🏪 행복 편의점 (Store)
![Store_Inside](https://github.com/user-attachments/assets/3840b3a3-d2f1-47e7-8810-d26614ebc4ce)
* **스트레스 해소**: 비용 **10만 원** 지출 시 맛있는 음식을 먹고 **스트레스 30을 해소**합니다.

---

## 🕹️ 추가 이벤트

### 📝 경제 상식 퀴즈 (Quiz)
![Quiz_Panel](https://github.com/user-attachments/assets/6b1b6df0-ea40-4d21-97e8-04e90f83d6f7)
* **규칙**: 금융 상식 관련 **3지선다형** 객관식 문제 출제 (한 달에 한 번 참여 가능).
* **보상**: 정답 시 **10만 원** 지급 / 오답 시 보너스 없음.

### 🚗 돌발 이벤트: 교통사고
![Accident_Panel](https://github.com/user-attachments/assets/a218cd1e-6ead-46f0-b2e5-02010bb86fa6)
* **발생**: 전체 맵 이동 중 무작위로 발생합니다.
* **페널티**: 현금 **-20만 원** / 스트레스 **+20**.
* **위험**: 자산이나 스트레스가 한계치인 상태에서 사고 시 즉시 게임 오버됩니다.

---

## 🏆 엔딩 및 스코어 (Ending & Score)

### **스코어 계산 방식**
최종 점수 = 총 자산

### **엔딩 종류**
* **🎉 성공 (Success)**: 10개월을 무사히 마치고 자산을 정산받는 엔딩.
* **💸 머니 아웃 (Money Out)**: 현금이 0원 미만이 되어 파산하는 엔딩.
* **😫 스트레스 오버 (Stress Over)**: 스트레스가 100%에 도달하여 쓰러지는 엔딩.

---

## 🖼️ 게임 갤러리

| 시작 화면 | 
| :---: |
| ![Start](https://github.com/user-attachments/assets/1f4cb485-1f6b-400b-bfac-5070ea29dd05) |

| 아카데미 내부 | 은행 내부 | 퀴즈 건물 내부 |
| :---: | :---: | :---: |
| ![Office_In](https://github.com/user-attachments/assets/00737616-048c-4098-922b-867299ae65f0) | ![Bank_In](https://github.com/user-attachments/assets/32ca3141-dfed-440d-ae0d-7d7c8a39fb2d) | ![Quiz_In](https://github.com/user-attachments/assets/e8f2690e-9fd5-408b-99eb-dbf91ac63a12) |

| 먹거리 건물 | 집 내부 | 
| :---: | :---: | 
| ![Store_Build](https://github.com/user-attachments/assets/6c76e142-e083-48f5-ba16-dbff9a41aa98) | ![Home_In](https://github.com/user-attachments/assets/8075e4ce-aeb0-4d1a-9196-2bbf36ec3b9e) |

---

## 🛠️ 개발 정보 (Tech Stack)
* **Engine**: Unity 2D
* **Language**: C#
* **UI**: TextMeshPro 기반 HUD 시스템
