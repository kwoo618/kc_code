# 💰 사회초년생의 자산 관리 시뮬레이션 (Asset Management Sim)

![Game_Start_Screen](https://github.com/user-attachments/assets/1f4cb485-1f6b-400b-bfac-5070ea29dd05)

> **"10개월의 시간, 당신은 얼마나 많은 자산을 모을 수 있습니까?"** > 사회초년생이 되어 월급 관리, 자기계발, 금융 활동을 통해 최고의 자산가에 도전해보세요!

---

## 📝 게임 소개 (Introduction)
본 게임은 이제 막 사회에 첫발을 내딛은 **사회초년생의 현실적인 경제 활동**을 담은 2D 시뮬레이션 게임입니다. 플레이어는 10개월이라는 한정된 시간 동안 매달 지급되는 월급을 효율적으로 관리하고, 자기계발을 통해 몸값을 올리며, 예기치 못한 사고와 스트레스에 대처해야 합니다. 단순한 저축을 넘어 대출과 적금 등 금융 시스템을 활용하여 최적의 자산을 모으는 것이 목표입니다.

## ✨ 주요 특징 (Key Features)
* **성장형 시스템**: 아카데미 교육을 통해 직무 레벨을 올리고 월급을 인상시키는 '자기계발' 요소 반영.
* **현실적인 금융 로직**: 정기적금의 강제 저축 기능과 대출 이자 시스템을 통한 전략적 자산 관리.
* **리스크 관리**: 무작위로 발생하는 교통사고와 매달 누적되는 스트레스 수치를 관리하는 긴장감 있는 플레이.
* **멀티 엔딩**: 최종 자산 상태뿐만 아니라 파산, 건강 악화(스트레스 오버) 등 플레이 결과에 따른 다양한 결말.

---

## 🕹️ HUD 및 시스템 대시보드 (HUD System)

![HUD_Panel](https://github.com/user-attachments/assets/9c271854-7d8b-446a-826d-7bc92d713fa1)

게임 화면 상단에서 실시간으로 자산 및 상태를 확인할 수 있습니다 (왼쪽부터 순서대로).

1. **📅 기간**: 현재 게임 진행 달수를 나타내며, 총 10개월 동안 진행됩니다.
2. **💵 현금**: 현재 보유한 현금입니다. '나의 집'에서 턴을 넘길 때 월급을 받거나, 퀴즈 정답 보상으로 늘릴 수 있습니다.
3. **📉 빚**: 은행에서 대출을 받으면 늘어납니다. 상환을 통해 줄일 수 있으며, 미상환 시 이자가 발생합니다.
4. **💼 월급**: 매달 받는 기본 수익입니다. 아카데미에서 자기계발을 통해 인상할 수 있습니다.
5. **💰 적금**: 은행에서 가입 가능하며, 가입 시 매달 자동으로 일정 금액이 저축됩니다.
6. **😫 스트레스**: 활동 시 누적되는 수치입니다. 100% 도달 시 게임 오버되므로 세심한 관리가 필요합니다.

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
* **월급 인상**: 직무 레벨 **1 상승당 월급 20만 원 인상** (최대 200만 원 추가 지급).
* **실무 강의 수강**: 비용 **15만 원** / 직무 레벨 **+1** / 스트레스 **+10**
* **실무 참여하기**: 비용 **100만 원** / 직무 레벨 **+3** / 스트레스 **+40**

### 🏦 은행 (Bank)
![Bank_Panel](https://github.com/user-attachments/assets/247531ad-8e4e-4551-bd7c-0a0755438eaa)
* **대출/상환**: 최대 200만 원까지 대출 가능하며, 50만 원 단위로 상환할 수 있습니다.
* **정기적금**: 가입 시 매달 50만 원이 자동이체되어 저축됩니다.

### 🏠 나의 집 (Home)
![Home_Inside](https://github.com/user-attachments/assets/e48c532f-022e-4695-87e9-1d29c17296db)
* **다음 달로 이동**: 한 달을 마무리하고 **월급 명세서**를 수령하며 턴을 넘깁니다.
* ※ 이동 시 업무 부담으로 인해 **스트레스가 40씩 증가**합니다.

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

## 🚀 설치 및 실행 방법 (Installation)

1. [Releases](https://github.com/사용자계정/저장소이름/releases) 페이지에서 최신 버전의 파일을 다운로드합니다.
2. 다운로드한 압축 파일을 해제합니다.
3. `AssetManagementSim.exe`를 실행하여 게임을 시작합니다.

---

## 🛠️ 개발 정보 (Tech Stack)
* **Engine**: Unity 2D
* **Language**: C#
* **UI**: TextMeshPro 기반 HUD 시스템
