create schema COREAPI
go

create table COREAPI.tblCustomer
(
Id bigint not null primary key,
	UserID	bigint not null,
	Name	varchar(100) not null,
	FullName	varchar(100) not null,
	DateofBirth	date not null
)

drop table COREAPI.tblCustomer

alter table COREAPI.tblCustomer add   Id bigint not null default(-1)

insert into COREAPI.tblCustomer values (1000, 'Kannan','Raja', '17 sep 1985')

select * from COREAPI.tblCustomer