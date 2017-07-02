#include "stdafx.h"
#include <windows.h>
#include <fstream>
#include <Shellapi.h>

#pragma comment(lib,"kernel32")
#pragma comment(lib,"user32")
#pragma comment(lib,"shell32")

#ifdef _WIN64
int mGWL_WNDPROC = GWLP_WNDPROC;
#else
int mGWL_WNDPROC = GWL_WNDPROC;
#endif
#define MAX_SIZE 40

RECT g_rcRebar;
HWND g_trayWnd;
HWND g_rebar;
WNDPROC g_oldProc = NULL;


void Writelog(LPSTR content)
{
	std::fstream f("d:\\1.txt", std::ios_base::app);
	f << content << std::endl;
	f.close();
}

LRESULT CALLBACK NewWndProc(HWND hWnd, UINT wMessage, WPARAM wParam, LPARAM lParam)
{
	if (wMessage == WM_WINDOWPOSCHANGING)
	{
		LPWINDOWPOS wndPos = (LPWINDOWPOS)lParam;
		GetWindowRect(g_rebar, &g_rcRebar);

		_AppBarData taskbarData;
		SHAppBarMessage(0x00000005, &taskbarData);
		int size;

		switch (taskbarData.uEdge)
		{
		case ABE_LEFT:
		case ABE_RIGHT:
			size = g_rcRebar.right - g_rcRebar.left;
			if (size > MAX_SIZE)
				size = MAX_SIZE;
			wndPos->y += size;
			wndPos->cy -= size;
			break;
		case ABE_TOP:
		case ABE_BOTTOM:
			size = g_rcRebar.bottom - g_rcRebar.top;
			if (size > MAX_SIZE)
				size = MAX_SIZE;
			wndPos->x += size;
			wndPos->cx -= size;
			break;
		}

		return CallWindowProc(g_oldProc, hWnd, wMessage, wParam, lParam);
	}
	else
	{
		if (NULL == g_oldProc)
			return DefWindowProc(hWnd, wMessage, wParam, lParam);
		else
			return CallWindowProc(g_oldProc, hWnd, wMessage, wParam, lParam);
	}
}

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
	)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		g_trayWnd = ::FindWindow(L"Shell_TrayWnd", NULL);
		g_rebar = ::FindWindowEx(g_trayWnd, NULL, L"ReBarWindow32", NULL);

		g_oldProc = (WNDPROC)::SetWindowLongPtr(g_rebar, mGWL_WNDPROC, (LONG_PTR)NewWndProc);
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		if (NULL != g_oldProc)
			SetWindowLongPtr(g_rebar, mGWL_WNDPROC, (LONG_PTR)g_oldProc);
		g_oldProc = NULL;
		break;
	}
	return TRUE;
}

