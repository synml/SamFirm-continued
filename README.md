# SamFirm-continued

> SamFirm의 연장 지원 프로젝트입니다.

SamFirm은 삼성 스마트폰의 순정 펌웨어를 다운받을 수 있는 프로그램입니다. [XDA 포럼](https://forum.xda-developers.com/galaxy-tab-s/general/tool-samfirm-samsung-firmware-t2988647)에 처음 게시되었고, 저를 포함한 많은 사용자에게 큰 도움이 되었습니다.

그러나 2016년 이후로 업데이트가 끊기면서 .NET Framework 4 이상을 지원하지 않으며, 최신 이외의 펌웨어는 다운로드가 불가능한 문제로 인해 [updato.com](https://updato.com/)이라는 사이트를 대신 이용하게 되었습니다. 이 사이트는 각 기종에 대한 펌웨어를 아카이빙한 것으로, 현재까지 출시된 펌웨어를 회원가입, 속도제한 없이 다운로드 할 수 있지만 최신 펌웨어 반영이 미흡한 것이 흠입니다.

따라서 [GitHub에 게시된 소스코드](https://github.com/eladkarako/SamFirm-Source)를 이용해 .NET Framework를 최신버전으로 변경하고, 작동불능 기능이나 불필요한 기능은 삭제하며 기존의 어려운 사용법을 최대한 쉽게 만들기로 했습니다.

이 프로젝트가 많은 사용자에게 유용히 사용되면 좋겠습니다. 😎

## 프로젝트 소개

- 동기
  - SamFirm 프로그램을 현재에도 원활히 사용할 수 있도록 개량하기 위해 시작했습니다.
- 목적
  - 최신 순정 펌웨어를 쉽게 다운로드 할 수 있다.
- 주요 기능
  - 최신 펌웨어 확인, 암호화 파일 다운로드, 복호화

## Build Status

[![release](https://img.shields.io/github/release/Lulin-Pollux/SamFirm-continued.svg?style=popout-square)](https://github.com/Lulin-Pollux/SamFirm-continued/releases/latest) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/47cf571666ef46a09a8087f18daed6d4)](https://www.codacy.com/app/Lulin-Pollux/SamFirm-continued?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Lulin-Pollux/SamFirm-continued&amp;utm_campaign=Badge_Grade) ![last commit](https://img.shields.io/github/last-commit/Lulin-Pollux/SamFirm-continued.svg?style=popout-square) ![](https://img.shields.io/github/downloads/Lulin-Pollux/SamFirm-continued/total.svg?style=popout-square) ![License](https://img.shields.io/github/license/Lulin-Pollux/SamFirm-continued.svg?style=popout-square) 

## 설치 방법

1. Release 뱃지를 클릭하여 릴리즈 페이지로 이동한다.
2. 가장 최신 버전을 다운로드한다. (Latest Release)
3. .NET Framework 4.7, [Visual C++ 2010 x86](http://www.microsoft.com/ko-kr/download/details.aspx?id=5555), [Visual C++ 2008 x86](https://www.microsoft.com/ko-kr/download/details.aspx?id=5582)가 설치되어있는지 확인해주세요. 누락된 부분은 구글링이나 각 링크를 클릭하여 설치파일을 다운로드 받은 후 설치하세요.

## 사용 예시

현재 베타버전이라 예전 SamFirm 프로그램의 사용법과 동일합니다. 정식버전 게시 이후 작성할 예정입니다.

## 기능

- 각 기종에 대한 최신 펌웨어 확인
- 원클릭 최신 펌웨어 다운로드 (이어받기 가능)
- CRC32 검사
- 자동 복호화

## API, 프레임워크

- .NET Framework 4.7.2
- NuGet 패키지
  - [Microsoft-WindowsAPICodePack-Core](https://www.nuget.org/packages/Microsoft-WindowsAPICodePack-Core/) 1.1.3.3
  - [Microsoft-WindowsAPICodePack-Shell](https://www.nuget.org/packages/Microsoft-WindowsAPICodePack-Shell/) 1.1.3.3
- DLL
  - AgentModule.dll
  - CommonModule.dll
  - GlobalUtil.dll

## 개발 환경

- S/W 개발 환경
  - Visual Studio 2017 Community (15.9.5)
  - .NET Framework 4.7.2
  - C# Language (x86 Build)
- 개발 환경 설정
  1. 리포지토리를 클론, 포크하거나 압축파일로 코드를 다운로드하세요.
  2. .NET Framework 4.7.2 개발 도구가 설치되어 있는지 확인하세요. 없으면 설치.
  3. Visual Studio 2017로 솔루션 파일(.sln)을 여세요.
  4. 코딩 시작~!

## 변경 로그

- 0.1.0
  - 첫 베타 릴리즈

## 개발자 정보와 크레딧

- 개발자
  - Lulin Pollux - [GitHub 프로필](https://github.com/Lulin-Pollux), [kistssy+dev@gmail.com](mailto:kistssy+dev@gmail.com)
- 크레딧
  - 원본 프로젝트: [eladkarako/SamFirm-Source](https://github.com/eladkarako/SamFirm-Source)
  - XDA Forum: [forum.xda-developers.com/...](https://forum.xda-developers.com/galaxy-tab-s/general/tool-samfirm-samsung-firmware-t2988647) (zxz0O0)

## 기여 방법

1. [이 프로젝트](https://github.com/Lulin-Pollux/SamFirm-continued)를 포크합니다.
2. GitHub Desktop에서 새 브랜치를 만드세요.
3. 수정사항을 커밋하세요.
4. 새 브랜치에 푸시하세요.
5. 풀리퀘스트를 보내주세요.

## 라이센스

MIT License © Lulin Pollux

`LICENSE`에서 자세한 정보를 확인할 수 있습니다.