import { Header, Logo, TabsNav, ThemeSwitch } from "@lobehub/ui";
import { memo, useEffect, useState } from "react";
import Avatar from "./Avatar";
import { useNavigate } from "react-router-dom";
import { useActiveTabKey } from "@/hooks/useActiveTabKey";
import { useUserStore } from "@/store/user";
import { Flexbox } from 'react-layout-kit';

const Top = memo(() => {
    const navigate = useNavigate();
    const activeTabKey = useActiveTabKey();
    const [isSignedIn, theme, setTheme] = useUserStore((s) => [s.isSignedIn, s.theme, s.setTheme]);

    const [tabs, setTabs] = useState([
        {
            key: 'packages',
            order: 1,
            label: '包',
        },
        {
            key: 'docs',
            order: 5,
            label: '文档',
        }]);

    useEffect(() => {
        if (isSignedIn && tabs.length === 2) {
            const items = [
                {
                    key: 'upload',
                    label: '上传',
                    order: 2,
                },
                {
                    key: 'key-manager',
                    label: '密钥管理',
                    order: 3,
                },
                {
                    key: 'current-package',
                    label: '包管理',
                    order: 4,
                }]

            setTabs((prev) => [...prev, ...items]);
        }
    }, [isSignedIn]);

    return (
        <Header
            logo={<Logo style={{
                cursor: 'pointer',
            }} extra={'NuGet Next'} onClick={() => {
                navigate('/');
            }} />}
            title="NuGet Next"
            nav={<>
                <TabsNav
                    activeKey={activeTabKey}
                    onChange={(key) => {
                        navigate("/" + key);
                    }}
                    items={tabs.sort((a, b) => a.order - b.order)}
                />
            </>}
            actions={<>
                <Flexbox horizontal>
                    <ThemeSwitch style={{
                        marginRight: 16,
                    }} onThemeSwitch={setTheme} themeMode={theme} />
                    <Avatar />
                </Flexbox>
            </>}
        >

        </Header>
    )
});

Top.displayName = 'Header'

export default Top;