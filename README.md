# 라들: 한글 단어 퍼즐 게임

'라들'은 단어 맞추기 게임인 Wordle의 한글 버전으로, Unity 2D를 이용하여 개발된 프로젝트입니다. 

이 게임은 사용자가 주어진 글자 수 내에서 정확한 단어를 조합하여 맞추어야 하는 간단 퍼즐 게임입니다.

### - 주요 특징 -

조합형 한글 입력: 모든 한글 조합이 가능한 커스텀 키보드 구현

Firebase 연동: 정답 단어가 Firestore 데이터베이스에 존재할 경우만 정답 확인 가능

힌트 시스템: 글자 칸과 키보드의 색 변화를 통해 초성, 중성, 종성에 대한 힌트 제공

가변적 단어 길이: 난이도에 따라 단어의 길이 조정 가능

유저 인증: 로그인, 회원가입, 게스트 로그인 지원 및 게스트에서 회원 전환 기능

점수 및 랭킹: 사용자 간 점수 경쟁과 랭킹 확인 가능

인증 메일 재발송: 60초 간격으로 인증 메일 재발송 가능

### - 기술 스택 -

프론트엔드: Unity 2D

백엔드: Firebase, Firestore

기타 도구: GitHub, Visual Studio
