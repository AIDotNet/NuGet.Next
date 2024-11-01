
import { Flexbox } from 'react-layout-kit';
import { Button, message, Table } from 'antd';
import Divider from '@lobehub/ui/es/Form/components/FormDivider';
import { useEffect, useState } from 'react';
import { Avatar } from '@lobehub/ui';
import { deleteUser, getUser } from '@/services/UserService';
import CreateUser from './features/CreateUser';


const UserManagement = () => {
    const columns = [
        {
            title: '用户名',
            dataIndex: 'username',
            key: 'username',
        },
        {
            title: '昵称',
            dataIndex: 'fullName',
            key: 'fullName',
        },
        {
            title: '邮箱',
            dataIndex: 'email',
            key: 'email',
        }, {
            title: '头像',
            dataIndex: 'avatar',
            key: 'avatar',
            render: (avatar: any) => {
                return (
                    <Avatar shape='circle' animation={true} avatar={avatar} />
                )
            }
        },
        {
            title: '角色',
            dataIndex: 'role',
            key: 'role',
        },
        {
            title: '操作',
            dataIndex: 'operation',
            key: 'operation',
            width: '100px',
            render: (_: any, item: any) => {
                return (
                    <Flexbox>
                        <Button style={{
                            marginBottom: '5px'
                        }}>编辑</Button>
                        <Button onClick={() => {
                            removeUser(item.id);
                        }} danger>删除</Button>
                    </Flexbox>
                )
            }
        }
    ];

    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);
    const [total, setTotal] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [createUserVisible, setCreateUserVisible] = useState(false);


    function removeUser(id: string) {
        deleteUser(id).then((res) => {
            if (res.success) {
                message.success("删除成功");
                loadData();
            } else {
                message.error(res.message);
            }
        }).catch(() => {
            message.error("删除失败");
        })
    }

    function loadData() {
        setLoading(true);
        try {
            getUser(undefined, page, pageSize).then(result => {
                setData(result.items);
                setTotal(result.total);
            });
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadData();
    }, [page, pageSize]);


    return (
        <Flexbox>
            <Flexbox horizontal>
                <span style={{
                    fontSize: '1.5rem',
                    fontWeight: 'bold',
                }}>
                    用户管理
                </span>
                <Button
                    onClick={() => {
                        setCreateUserVisible(true);
                    }}
                    style={{
                        marginLeft: 'auto'
                    }}>
                    新增用户
                </Button>
            </Flexbox>
            <Divider />
            <Table columns={columns} dataSource={data} loading={loading} pagination={{
                current: page,
                pageSize: pageSize,
                total: total,
                onChange: (page, pageSize) => {
                    setPage(page);
                    setPageSize(pageSize);
                }
            }} />
            <CreateUser open={createUserVisible} onClose={() => setCreateUserVisible(false)} onSubmit={() => {
                loadData();
                setCreateUserVisible(false);
            }} />
        </Flexbox>
    )
}


export default UserManagement;